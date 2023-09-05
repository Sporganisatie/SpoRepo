
using Microsoft.EntityFrameworkCore;
using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Models;
using SpoRE.Models.Response;

namespace SpoRE.Services;

public class RaceService
{
    private readonly RaceClient RaceClient;
    private readonly Userdata User;
    private readonly TeamSelectionClient TeamClient;
    private readonly DatabaseContext DB;

    public RaceService(RaceClient raceClient, Userdata userData, TeamSelectionClient teamClient, DatabaseContext databaseContext)
    {
        RaceClient = raceClient;
        User = userData;
        TeamClient = teamClient;
        DB = databaseContext;
    }

    internal RaceState GetRaceState(int raceId)
    {
        var participationCount = DB.AccountParticipations.Count(x => x.AccountId == User.Id && x.RaceId == raceId);
        // if race finished => race samenvatting pagina
        if (RaceClient.ShowResults(raceId, 1)) return new(RaceStateEnum.Started, RaceClient.CurrentStage(raceId)?.Stagenr ?? DB.Stages.Count(s => s.RaceId == raceId));

        var stateBeforeStart = DB.AccountParticipations.Count(x => x.AccountId == User.Id && x.RaceId == raceId) > 0
                ? RaceStateEnum.TeamSelection
                : RaceStateEnum.NotJoined;
        return new(stateBeforeStart, 0);
    }

    public Task<Result> JoinRace(int raceId)
        => TeamClient.JoinRace(raceId);

    public int SetFinished(int raceId)
    {
        var race = DB.Races.Single(r => r.RaceId == raceId);
        race.Finished = true;
        var participations = DB.AccountParticipations.Where(ap => ap.RaceId == raceId).Select(ap => ap.AccountParticipationId).ToList();
        foreach (var apId in participations)
        {
            var stageSelectionId = DB.StageSelections
                .FromSqlInterpolated($@"
                    SELECT stage_selection_id 
                    FROM stage_selection 
                    INNER JOIN stage USING(stage_id)
                    WHERE account_participation_id = {apId} AND type = 'FinalStandings'")
                .Select(ss => ss.StageSelectionId)
                .FirstOrDefault();

            var finalScore = DB.StageSelections
                .Where(ss => ss.StageSelectionId == stageSelectionId)
                .Select(ss => ss.TotalScore)
                .FirstOrDefault();

            var accountParticipation = DB.AccountParticipations.Find(apId);
            if (accountParticipation != null)
            {
                accountParticipation.FinalScore = finalScore;
                DB.AccountParticipations.Update(accountParticipation);
            }
        }

        return DB.SaveChanges();
    }
}