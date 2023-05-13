namespace SpoRE.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

public class MissedPointsData
{
    public int Etappe { get; set; }
    public int Behaald { get; set; }
    public int Optimaal { get; set; }
    public int Gemist { get; set; }
}

public class MissedPointsTable
{
    public List<MissedPointsData> Data { get; set; }
    public string Username { get; set; }
}

public partial class StatisticsService
{
    private readonly DatabaseContext DB;

    public StatisticsService(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }
    internal object MissedPoints(int raceId, bool budgetParticipation)
        => DB.AccountParticipations.Include(ss => ss.Account)
            .Where(ss => ss.RaceId == raceId && ss.Budgetparticipation == budgetParticipation)
            .Select(MissedPointsRider);

    private MissedPointsTable MissedPointsRider(AccountParticipation user)
    {
        return new() { Username = user.Account.Username };
    }
}