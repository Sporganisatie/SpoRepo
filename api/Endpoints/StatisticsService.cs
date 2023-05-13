namespace SpoRE.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

public record MissedPointsData(int Etappe, int Behaald, int Optimaal, int Gemist);

public record MissedPointsTable(string Username, List<MissedPointsData> Data);

public partial class StatisticsService
{
    private readonly DatabaseContext DB;

    public StatisticsService(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    internal IEnumerable<MissedPointsTable> MissedPoints(int raceId, bool budgetParticipation)
        => DB.AccountParticipations.Include(ss => ss.Account)
            .Where(ss => ss.RaceId == raceId && ss.Budgetparticipation == budgetParticipation).ToList()
            .Select(MissedPointsUser);

    public MissedPointsTable MissedPointsUser(AccountParticipation user)
    {
        var teamSelection = DB.TeamSelections
            .Where(tsr => tsr.AccountParticipationId == user.AccountParticipationId)
            .Select(tsr => tsr.RiderParticipationId).ToList();


        var ridersResults = DB.ResultsPoints
            .Join(DB.Stages, rp => rp.StageId, s => s.StageId, (rp, s) => new { Result = rp, Stage = s })
            .Where(joinedData => teamSelection.Contains(joinedData.Result.RiderParticipationId))
            .GroupBy(joinedData => joinedData.Stage.Stagenr)
            .Select(groupedData => new
            {
                Stagenr = groupedData.Key,
                Points = groupedData.Select(g => new
                {
                    Id = g.Result.RiderParticipationId,
                    Stage = g.Result.Stagescore,
                    Total = user.Budgetparticipation ? g.Result.Totalscore - g.Result.Teamscore : g.Result.Totalscore
                })
                .OrderByDescending(g => g.Total)
                .ToList()
            })
            .ToList();


        var actualScores = DB.StageSelections.Include(ss => ss.Stage).Where(ss => ss.AccountParticipationId == user.AccountParticipationId)
                .ToList().Where(ss => ss.Stage.Starttime < DateTime.UtcNow);

        var missedPoints = new List<MissedPointsData>();
        foreach (var riders in ridersResults)
        {
            var actualScore = actualScores.Single(a => a.Stage.Stagenr == riders.Stagenr).Stagescore ?? 0;
            var optimalKopmanPoints = riders.Points.Max(r => r.Stage) * 0.5;
            // TODO check of optimal kopman in eerste 9
            var optimalPoints = (int)(riders.Points.Take(9).Sum(r => r.Total) + optimalKopmanPoints);
            missedPoints.Add(new(riders.Stagenr, actualScore, optimalPoints, optimalPoints - actualScore));
        }
        return new(user.Account.Username, missedPoints);
        // var totalQuery = $"{ridersQuery}\n{resultsQuery}";

        // var results = await DB.Database.ExecuteSqlRawAsync(totalQuery);

        // var outputArray = new List<Dictionary<string, object>>();
        // var actualPoints = results[1].Rows.Select(a => a.stagescore).ToList();
        // var optimalTotal = 0;
        // var actualTotal = 0;
        // var missedTotal = 0;

        // for (var i = 0; i < results[0].Rows.Count; i++)
        // {
        //     var optimalPoints = 0;
        //     var totalScores = results[0].Rows[i].points.Select(scores => new { score = scores.total, id = scores.id }).ToList();
        //     var stageScores = results[0].Rows[i].points.Select(scores => new { score = scores.stage, id = scores.id }).ToList();
        //     stageScores = stageScores.OrderByDescending(a => a.score).ToList();
        //     var bestId = stageScores[0].id;
        //     var pos = AttrIndex(totalScores, "index", bestId);
        //     var forRenners = 9;

        //     if (pos > 8)
        //         forRenners = 8;

        //     for (var j = 0; j < forRenners; j++)
        //     {
        //         if (totalScores[j] == null)
        //             continue;

        //         optimalPoints += totalScores[j].score;

        //         if (totalScores[j].id == bestId)
        //         {
        //             optimalPoints += stageScores[0].score * 0.5;
        //         }
        //     }

        //     if (forRenners == 8)
        //     {
        //         outputArray.Add(new Dictionary<string, object>
        //     {
        //         { "Behaald", "Zeg tegen Rens" },
        //         { "Optimaal", "dat er iets" },
        //         { "Gemist", "speciaals gebeurt is" }
        //     });
        //     }
        //     else
        //     {
        //         if (i == 21)
        //         {
        //             outputArray.Add(new Dictionary<string, object>
        //         {
        //             { "Etappe", i + 1 },
        //             { "Behaald", actualPoints[i] },
        //             { "Optimaal", actualPoints[i] },
        //             { "Gemist", 0 }
        //         });
        //         }
        //         else
        //         {
        //             outputArray.Add(new Dictionary<string, object>
        //         {
        //             { "Etappe", i + 1 },
        //             { "Behaald", actualPoints[i] },
        //             { "Optimaal", optimalPoints },
        //             { "Gemist", optimalPoints - actualPoints[i]}
        //         });
        //         }
        //         optimalTotal += optimalPoints;
        //         actualTotal += actualPoints[i];
        //         missedTotal += optimalPoints - actualPoints[i];
        //     }
        // }
        // outputArray.Add(new Dictionary<string, object>
        //         {
        //             { "Etappe", "Totaal" },
        //             { "Behaald", actualTotal },
        //             { "Optimaal", optimalTotal },
        //             { "Gemist", missedTotal}
        //         });
    }

    private MissedPointsData MissedPointsStage(object r, StageSelection stageSelection)
    {
        throw new NotImplementedException();
    }
}