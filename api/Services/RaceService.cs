
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Models.Response;
using static SpoRE.Helper.HelperFunctions;

namespace SpoRE.Services;

public class RaceService(Userdata User, DatabaseContext DB, IMemoryCache MemoryCache)
{
    internal RaceState GetRaceState(int raceId)
    {
        if (DB.ShowResults(raceId, 1))
        {
            var currentStage = DB.Stages.Where(s => s.RaceId == raceId && !s.Complete).OrderBy(s => s.Starttime).FirstOrDefault();
            var currentStagenr = currentStage?.Stagenr ?? DB.Stages.Count(s => s.RaceId == raceId);
            return new(RaceStateEnum.Started, currentStagenr);
        }

        var stateBeforeStart = DB.AccountParticipations.Any(x => x.AccountId == User.Id && x.RaceId == raceId)
                ? RaceStateEnum.TeamSelection
                : RaceStateEnum.NotJoined;
        return new(stateBeforeStart, 0);
    }

    public int SetFinished(int raceId)
    {
        var race = DB.Races.Single(r => r.RaceId == raceId);
        race.Finished = true;
        var participations = DB.AccountParticipations.Where(ap => ap.RaceId == raceId).ToList();
        foreach (var ap in participations)
        {
            var finalScore = DB.StageSelections.Include(ss => ss.Stage)
                .Single(ss => ss.Stage.IsFinalStandings && ss.AccountParticipationId == ap.AccountParticipationId)
                .TotalScore;

            ap.FinalScore = finalScore;
            DB.AccountParticipations.Update(ap);
        }

        ((MemoryCache)MemoryCache).Clear();
        return DB.SaveChanges();
    }

    public int JoinRace(int raceId)
    {
        string budgetInsert = User.Id <= 5 ? $",({User.Id},{raceId},true)" : "";

        string accountParticipationQuery = $@"INSERT INTO account_participation(account_id, race_id, budgetparticipation)
                VALUES({User.Id}, {raceId}, false) {budgetInsert}
                ON CONFLICT (account_id, race_id, budgetparticipation) DO UPDATE SET budgetparticipation = EXCLUDED.budgetparticipation";

        _ = DB.Database.ExecuteSqlRaw(accountParticipationQuery);
        string stage_selectionQuery = "INSERT INTO stage_selection(stage_id, account_participation_id) VALUES";
        for (int stage = 1; stage < 23; stage++)
        {
            string stage_id = $"(SELECT stage_id FROM stage WHERE race_id = {raceId} AND stagenr = {stage})";
            string budgparticipationInsert = "";
            if (User.Id <= 5)
            {
                budgparticipationInsert = $",({stage_id},(SELECT account_participation_id FROM account_participation WHERE race_id = {raceId} AND account_id = {User.Id} AND budgetparticipation = true))";
            }
            stage_selectionQuery += $"({stage_id},(SELECT account_participation_id FROM account_participation WHERE race_id = {raceId} AND account_id = {User.Id} AND budgetparticipation = false)){budgparticipationInsert},";
        }

        stage_selectionQuery = stage_selectionQuery.Remove(stage_selectionQuery.Length - 1) + " ON CONFLICT (account_participation_id, stage_id) DO NOTHING;";
        return DB.Database.ExecuteSqlRaw(stage_selectionQuery);
    }

    public TeamSelections AllTeamSelections(int raceId, bool budgetParticipation)
    {
        var teamSelection = DB.AccountParticipations.Include(ap => ap.RiderParticipations).Single(ts => ts.AccountParticipationId == User.ParticipationId).RiderParticipations;
        var users = DB.AccountParticipations.Include(ap => ap.Account).Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation).Select(ss => new { ss.AccountParticipationId, ss.Account.Username }).ToList();
        var results = DB.ResultsPoints.Where(rp => rp.Stage.RaceId == raceId).AsNoTrackingWithIdentityResolution().ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var stageSelections = DB.StageSelections.Include(ss => ss.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking().Where(ss => ss.AccountParticipationId == user.AccountParticipationId);
            var riderParticipations = stageSelections.SelectMany(ss => ss.RiderParticipations.Select(rp => new
            {
                RiderParticipation = rp,
                IsKopman = ss.KopmanId == rp.RiderParticipationId,
                ss.StageId
            })).ToList();

            var riderScores = riderParticipations.Join(
                results,
                rp => new { rp.RiderParticipation.RiderParticipationId, rp.StageId },
                res => new { res.RiderParticipationId, res.StageId },
                (rp, res) => new { rp.RiderParticipation, TotalScore = (rp.IsKopman ? res.StageScore * 0.5 : 0) + res.Totalscore - (budgetParticipation ? res.Teamscore : 0) }
            ).GroupBy(rp => rp.RiderParticipation).Select(rp => new StageComparisonRider
            {
                Rider = rp.Key.Rider,
                TotalScore = (int)rp.Sum(x => x.TotalScore),
                Selected = teamSelection.Contains(rp.Key) ? StageSelectedEnum.InStageSelection : StageSelectedEnum.None,
                Dnf = rp.Key.Dnf
            }).ToList().OrderByDescending(x => x.TotalScore);

            var nietOpgesteld = DB.AccountParticipations.Include(ap => ap.RiderParticipations).ThenInclude(rp => rp.Rider).AsNoTracking()
                .Single(ap => ap.AccountParticipationId == user.AccountParticipationId).RiderParticipations.Select(rp => new StageComparisonRider
                {
                    Rider = rp.Rider,
                    TotalScore = 0,
                    Selected = teamSelection.Contains(rp) ? StageSelectedEnum.InStageSelection : StageSelectedEnum.None,
                    Dnf = rp.Dnf
                }).Where(rp => !riderScores.Contains(rp));

            output.Add(new UserSelection(user.Username, riderScores.Union(nietOpgesteld), []));
        }
        return OrderSelectedRiders(output);
    }

    internal int Current()
        => DB.Races.OrderByDescending(x => x.RaceId).First(x => x.RaceId != 99).RaceId;

    internal IEnumerable<RaceSelection> AllRaces()
    {
        var query = from s in DB.Stages.Include(x => x.Race)
                    where s.RaceId != 99 && s.Race.Name != "classics"
                    group s.Race by s.Race into g
                    select g.Key;

        return query.OrderByDescending(x => x.Year).ThenByDescending(x => x.Name).ToList().Select(x => new RaceSelection($"{char.ToUpper(x.Name[0]) + x.Name[1..]} \t {x.Year}", x.RaceId));
    }
}

public record RaceSelection(string DisplayValue, int Value);