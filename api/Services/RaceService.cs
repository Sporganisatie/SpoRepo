
using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Models.Response;
using static SpoRE.Helper.HelperFunctions;

namespace SpoRE.Services;

public class RaceService
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;

    public RaceService(Userdata userData, DatabaseContext databaseContext)
    {
        User = userData;
        DB = databaseContext;
    }

    internal RaceState GetRaceState(int raceId)
    {
        var participationCount = DB.AccountParticipations.Count(x => x.AccountId == User.Id && x.RaceId == raceId);
        // if race finished => race samenvatting pagina
        if (DB.ShowResults(raceId, 1)) return new(RaceStateEnum.Started, DB.CurrentStage(raceId)?.Stagenr ?? DB.Stages.Count(s => s.RaceId == raceId));

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
                .Single(ss => ss.Stage.Type == StageType.FinalStandings && ss.AccountParticipationId == ap.AccountParticipationId)
                .TotalScore;

            ap.FinalScore = finalScore;
            DB.AccountParticipations.Update(ap);
        }

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
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var users = DB.AccountParticipations.Include(ap => ap.Account).Where(ap => ap.RaceId == raceId && ap.BudgetParticipation == budgetParticipation).Select(ss => new { ss.AccountParticipationId, ss.Account.Username }).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider).Where(ts => ts.AccountParticipationId == user.AccountParticipationId)
                        join rp in
                        (from rp in DB.ResultsPoints.Include(rp => rp.RiderParticipation.Rider)
                         join ssr in DB.StageSelectionRiders on rp.RiderParticipationId equals ssr.RiderParticipationId
                         join ss in DB.StageSelections on new { ssr.StageSelectionId, rp.StageId } equals new { StageSelectionId = ss.StageSelectionId, StageId = ss.StageId }
                         where ss.AccountParticipationId == user.AccountParticipationId
                         group new { rp, ss.KopmanId } by rp.RiderParticipation into g
                         select new
                         {
                             RiderParticipation = g.Key,
                             TotalScore = (int?)g.Sum(item => item.KopmanId == item.rp.RiderParticipationId ? item.rp.Totalscore + item.rp.StageScore * 0.5 : item.rp.Totalscore) - (budgetParticipation ? g.Sum(item => item.rp.Teamscore) : 0),
                         }) on ts.RiderParticipationId equals rp.RiderParticipation.RiderParticipationId into results
                        from rp in results.DefaultIfEmpty()
                        select new StageComparisonRider
                        {
                            Rider = ts.RiderParticipation.Rider,
                            TotalScore = rp == null ? 0 : (rp.TotalScore ?? 0),
                            Selected = teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : StageSelectedEnum.None,
                            Dnf = ts.RiderParticipation.Dnf
                        };

            var riderScores = query.ToList().OrderByDescending(x => x.TotalScore);
            output.Add(new UserSelection(user.Username, riderScores, new List<StageComparisonRider>()));
        }
        return OrderSelectedRiders(output);
    }
}