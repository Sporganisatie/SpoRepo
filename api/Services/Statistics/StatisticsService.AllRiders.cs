using System.Dynamic;
using Microsoft.EntityFrameworkCore;

namespace SpoRE.Services;

public partial class StatisticsService
{
    public IEnumerable<object> AllRiders(int raceId, bool budgetParticipation)
    {
        var query = from rp in DB.RiderParticipations.Include(rp => rp.Rider).Where(rp => rp.RaceId == raceId && rp.Price <= (budgetParticipation ? 750000 : int.MaxValue))
                    join rider in DB.Riders on rp.RiderId equals rider.RiderId
                    join repo in DB.ResultsPoints on rp.RiderParticipationId equals repo.RiderParticipationId into results
                    join ts in DB.TeamSelections.Include(ts => ts.AccountParticipation.Account).Where(ts => ts.AccountParticipation.BudgetParticipation == budgetParticipation) on rp.RiderParticipationId equals ts.RiderParticipationId into tsGroup
                    from ts in tsGroup.DefaultIfEmpty()
                    from points in results.DefaultIfEmpty()
                    group new { rp, rider, points, ts } by rp into g
                    select new
                    {
                        RiderParticipation = g.Key,
                        Rider = g.Select(x => x.rider).First(),
                        StageScore = g.Sum(x => x.points.StageScore) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        Klassementen = g.Sum(x => x.points.Gc.Score + x.points.Points.Score + x.points.Kom.Score + x.points.Youth.Score ?? 0) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TeamScore = g.Sum(x => x.points.Teamscore) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TotalScore = (budgetParticipation ? g.Sum(x => x.points.Totalscore) - g.Sum(x => x.points.Teamscore) : g.Sum(x => x.points.Totalscore)) / Math.Max(g.Select(x => x.ts.AccountParticipationId).Distinct().Count(), 1),
                        TotalSelected = g.Select(x => x.ts.AccountParticipationId).Distinct().Count(),
                        Accounts = g.Select(x => x.ts.AccountParticipation.Account.Username).Distinct()
                    };
        return query.OrderByDescending(x => x.TotalScore).ToList();
    }

    // TODO make DB calls without ef core for the situations where it creates really shitty queries like above
    // static string riderpointsallColumns(string userCount, bool budgetparticipation)
    // {
    //     string stageScore = $@"COALESCE(SUM(stagescore) / {userCount}, 0) AS ""Etappe""";
    //     string gcScore = $@"COALESCE(SUM(gcscore) / {userCount}, 0) AS ""AK""";
    //     string pointsScore = $@"COALESCE(SUM(pointsscore) / {userCount}, 0) AS ""Punten""";
    //     string komScore = $@"COALESCE(SUM(komscore) / {userCount}, 0) AS ""Berg""";
    //     string youthScore = $@"COALESCE(SUM(yocscore) / {userCount}, 0) AS ""Jong""";
    //     string klassementScore = $@"COALESCE((SUM(gcscore) + SUM(pointsscore) + SUM(komscore) + SUM(yocscore)) / {userCount}, 0) AS ""Klassement""";
    //     string teamscore = budgetparticipation ? "" : $@"SUM(teamscore) / {userCount} AS ""Team"",";
    //     string totalscoreVal = budgetparticipation ? @"totalscore - teamscore" : @"totalscore";
    //     string PPM = $@"COALESCE(ROUND(SUM({totalscoreVal}) / {userCount} * 1e6 / price, 0), 0) AS ""PPM""";
    //     string totalScore = $@"COALESCE(SUM({totalscoreVal}) / {userCount}, 0) AS ""Total""";
    //     string nameLink = $@"CONCAT('/rider/', rider_participation.rider_id) AS ""Name_link""";
    //     string riderName = $@"CONCAT(firstname, ' ', lastname) AS ""Name""";
    //     return $@"{nameLink}, {riderName}, team AS ""Team "", price AS ""Price"",
    // {stageScore}, {gcScore}, {pointsScore}, {komScore}, {youthScore},
    // {klassementScore}, {teamscore} {totalScore},
    // {PPM}, CASE WHEN dnf THEN 'DNF' ELSE '' END AS ""dnf"",";
    // }

    // public IEnumerable<object> FastQuery(int raceId, bool budgetParticipation)
    // {
    //     var userCount = "count(DISTINCT username)";
    //     string userCountNS = "1"; // Not Selected
    //     string budgetOnly = budgetParticipation ? "AND price <= 750000" : "";

    //     string notSelectedRiders = $@"UNION 
    //         SELECT {riderpointsallColumns(userCountNS, budgetParticipation)}
    //         0 AS ""Usercount"", '' AS ""Users"" FROM rider_participation 
    //         LEFT JOIN results_points USING (rider_participation_id) 
    //         INNER JOIN rider USING(rider_id) 
    //         WHERE rider_participation.race_id = {raceId} AND 
    //         NOT rider_participation_id IN (SELECT rider_participation_id 
    //         FROM team_selection_rider INNER JOIN account_participation USING (account_participation_id) 
    //         WHERE budgetparticipation = {budgetParticipation}) {budgetOnly} 
    //         GROUP BY ""Name"", ""Name_link"", ""Team "", ""Price"", dnf";

    //     string query = $@"SELECT {riderpointsallColumns(userCount, budgetParticipation)} 
    //         {userCount} AS ""Usercount"", string_agg(DISTINCT username, ', ') AS ""Users"" FROM rider_participation 
    //         LEFT JOIN results_points USING (rider_participation_id) 
    //         INNER JOIN rider USING(rider_id) 
    //         INNER JOIN team_selection_rider ON rider_participation.rider_participation_id = team_selection_rider.rider_participation_id 
    //         INNER JOIN account_participation USING(account_participation_id) 
    //         INNER JOIN account USING (account_id) 
    //         WHERE rider_participation.race_id = {raceId} AND 
    //         rider_participation.rider_participation_id IN (SELECT rider_participation_id FROM team_selection_rider) AND 
    //         budgetparticipation = {budgetParticipation} 
    //         GROUP BY ""Name"", ""Name_link"", ""Team "", ""Price"", dnf 
    //         {notSelectedRiders} 
    //         ORDER BY ""Total"" DESC";

    //     var a = DB.Database.ExecuteSqlRaw(query);
    //     return new List<object>();
    // }
}
