
using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public partial class StageResultService
{
    private readonly Userdata User;
    private readonly DatabaseContext DB;
    private readonly RaceClient RaceClient;

    public StageResultService(DatabaseContext databaseContext, Userdata userData, RaceClient raceClient)
    {
        DB = databaseContext;
        User = userData;
        RaceClient = raceClient;
    }

    public StageResultData StageResultData(int raceId, bool budgetParticipation, int stagenr)
    {
        if (!RaceClient.StageStarted(raceId, stagenr)) return new(new List<UserScore>(), new List<RiderScore>(), new Classifications(new List<ClassificationRow>(), new List<ClassificationRow>(), new List<ClassificationRow>(), new List<ClassificationRow>()));
        var userScores = GetUserScores(raceId, budgetParticipation, stagenr);
        var teamResult = GetTeamResult(raceId, stagenr, budgetParticipation);
        var classifications = GetClassifications(raceId, stagenr, top5: false);
        return new(userScores, teamResult, classifications);
    }

    public IEnumerable<UserSelection> AllStageSelections(int raceId, bool budgetParticipation, int? stagenr)
    {
        if (stagenr is null) return AllTeamSelections(raceId, budgetParticipation);
        if (DB.Stages.Single(s => s.RaceId == raceId && s.Stagenr == stagenr).Starttime > DateTime.UtcNow) return new List<UserSelection>();

        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var stageSelection = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipationId == User.ParticipationId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var users = DB.StageSelections.Include(ss => ss.AccountParticipation.Account).Where(ss => ss.Stage.Stagenr == stagenr && ss.Stage.RaceId == raceId && ss.AccountParticipation.Budgetparticipation == budgetParticipation).Select(ss => new { ss.StageSelectionId, ss.AccountParticipation.Account.Username, ss.AccountParticipationId }).ToList();
        var allSelected = DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.AccountParticipation.Budgetparticipation == budgetParticipation && ssr.StageSelection.Stage.RaceId == raceId && ssr.StageSelection.Stage.Stagenr == stagenr).Select(ssr => ssr.RiderParticipationId).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ssr in DB.StageSelectionRiders.Include(ssr => ssr.RiderParticipation.Rider)
                        join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ssr.RiderParticipationId equals rp.RiderParticipationId into results
                        from rp in results.DefaultIfEmpty()
                        where ssr.StageSelection.StageSelectionId == user.StageSelectionId
                        select new StageComparisonRider
                        {
                            Rider = ssr.RiderParticipation.Rider,
                            Kopman = ssr.RiderParticipationId == ssr.StageSelection.KopmanId,
                            StagePos = rp.Stagepos,
                            TotalScore = ((budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0) + (rp.RiderParticipationId == ssr.StageSelection.KopmanId ? (int)(rp.Stagescore * 0.5) : 0),
                            Selected = stageSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ssr.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                        };
            var riderScores = query.OrderBy(r => r.StagePos).ToList();
            var totals = new StageComparisonRider
            {
                TotalScore = riderScores.Sum(rs => rs.TotalScore)
            };

            var gemistQuery = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                              join rp in DB.ResultsPoints.Where(rp => rp.Stage.Stagenr == stagenr) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                              from rp in results.DefaultIfEmpty()
                              where (ts.AccountParticipationId == user.AccountParticipationId) && (rp.Totalscore > 0 || allSelected.Contains(ts.RiderParticipationId))
                                && !DB.StageSelectionRiders.Where(ssr => ssr.StageSelection.StageSelectionId == user.StageSelectionId).Any(ssr => ssr.RiderParticipationId == ts.RiderParticipationId)
                              select new StageComparisonRider
                              {
                                  Rider = ts.RiderParticipation.Rider,
                                  StagePos = rp.Stagepos,
                                  TotalScore = (budgetParticipation ? (rp.Totalscore - rp.Teamscore) : rp.Totalscore) ?? 0,
                                  Selected = stageSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InTeam : StageSelectedEnum.None
                              };

            output.Add(new UserSelection(user.Username, riderScores.Append(totals), gemistQuery.ToList()));
        }
        // order by totals
        return output.OrderByDescending(x => x.Riders.Last().TotalScore);
    }

    private IEnumerable<UserSelection> AllTeamSelections(int raceId, bool budgetParticipation)
    {
        var teamSelection = DB.TeamSelections.Where(ts => ts.AccountParticipationId == User.ParticipationId).Select(ts => ts.RiderParticipationId).ToList();
        var users = DB.AccountParticipations.Include(ap => ap.Account).Where(ap => ap.RaceId == raceId && ap.Budgetparticipation == budgetParticipation).Select(ss => new { ss.AccountParticipationId, ss.Account.Username }).ToList();
        var output = new List<UserSelection>();
        foreach (var user in users)
        {
            var query = from ts in DB.TeamSelections.Include(ts => ts.RiderParticipation.Rider)
                            // join rp in DB.ResultsPoints.Where(rp => rp.Stage.RaceId == raceId) on ts.RiderParticipationId equals rp.RiderParticipationId into results
                            // from rp in results.DefaultIfEmpty()
                        where ts.AccountParticipationId == user.AccountParticipationId
                        select new StageComparisonRider
                        {
                            Rider = ts.RiderParticipation.Rider,
                            TotalScore = 0,
                            Selected = teamSelection.Contains(ts.RiderParticipationId) ? StageSelectedEnum.InStageSelection : StageSelectedEnum.None
                        };
            var riderScores = query.ToList();
            var totals = new StageComparisonRider
            {
                TotalScore = riderScores.Sum(rs => rs.TotalScore)
            };
            output.Add(new UserSelection(user.Username, riderScores.Append(totals), new List<StageComparisonRider>()));
        }
        return output;
    }
}