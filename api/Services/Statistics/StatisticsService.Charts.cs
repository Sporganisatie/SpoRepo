using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public record LineChartData(IEnumerable<string> Users, IEnumerable<Dictionary<string, object>> Data);

public enum PositionScoringMethod
{
    AantalDeelnemersMinPositie,
    EenPuntPerOverwinning,
    Podium521,
    Podium421,
    Podium321
}

public partial class StatisticsService
{
    public LineChartData ScoreVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = (from uss in UserStageScores(raceId, budgetParticipation)
                         group uss by uss.StageNumber into stageScores
                         orderby stageScores.Key
                         select new Scores(stageScores.Select(x => new UsernameScore(x.Username, x.TotalScore.Value - (int)stageScores.Average(y => y.TotalScore))).ToList(), stageScores.Key.ToString())).ToList();

        var participants = GetParticipants(raceId, budgetParticipation);
        var start = new Scores(participants.Select(par => new UsernameScore(par, 0)).ToList(), "");

        return new(participants, uitslagen.Prepend(start).Select(x => ConvertToDict(x)));
    }

    private static Dictionary<string, object> ConvertToDict(Scores scores)
    {
        var dict = new Dictionary<string, object>
        {
            {"Name", scores.StageNumber}
        };
        foreach (var userscore in scores.UsernamesAndScores)
        {
            dict.Add(userscore.Username, userscore.Score);
        }
        return dict;
    }

    public LineChartData RaceScoreVerloop(bool budgetParticipation)
    {
        var subquery = from ap in DB.AccountParticipations.Include(ap => ap.Race)
                       where ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics" && ap.AccountId <= 5
                       select new
                       {
                           ap.Account,
                           ap.FinalScore,
                           ap.Race
                       };

        var perUser = from sq in subquery
                      group new { sq } by sq.Account into g
                      select new
                      {
                          g.Key,
                          scores = g.ToList().Select(sq => sq.sq)
                      };

        var ordered = perUser.ToList().Select(x => new { Username = x.Key, Scores = x.scores.OrderBy(x => x.Race.Year).ThenBy(x => x.Race.Name) });

        var modifiedList = new List<(Account Account, int? FinalScore, Race Race)>();
        foreach (var user in ordered)
        {
            int sum = 0;
            foreach (var item in user.Scores)
            {
                sum += item.FinalScore ?? 0;
                modifiedList.Add((item.Account, sum, item.Race));
            }
        }

        var uitslagen = modifiedList
            .GroupBy(ss => ss.Race)
            .Select(g => new Scores(
                g.OrderBy(ss => ss.Account.AccountId)
                 .Select(ss => new UsernameScore(ss.Account.Username, ss.FinalScore - ((int)g.Average(x => x.FinalScore)) ?? 0))
                 .ToList(),
                $"{g.Key.Year} {char.ToUpper(g.Key.Name[0]) + g.Key.Name[1..]}"))
            .ToList().OrderBy(x => x.StageNumber).ToList();

        var participants = uitslagen.First().UsernamesAndScores.Select(x => x.Username);

        var start = new Scores(participants.Select(x => new UsernameScore(x, 0)).ToList(), "");

        return new(participants, uitslagen.Prepend(start).Select(x => ConvertToDict(x)));
    }

    public LineChartData StagenummerScoreVerloop(bool budgetParticipation, bool genormaliseerd, bool totaalScore, int startRaceId, int endRaceId)
    {
        var rows = (from ss in DB.StageSelections
                    where ss.AccountParticipation.BudgetParticipation == budgetParticipation
                        && ss.Stage.RaceId != 99 && ss.Stage.Race.Name != "classics"
                        && ss.Stage.Finished
                    select new
                    {
                        ss.AccountParticipation.AccountId,
                        ss.AccountParticipation.Account.Username,
                        ss.Stage.StageId,
                        ss.Stage.Stagenr,
                        RaceYear = ss.Stage.Race.Year,
                        RaceName = ss.Stage.Race.Name,
                        Score = totaalScore ? ss.TotalScore : ss.StageScore
                    }).ToList();

        var startRace = DB.Races.AsNoTracking().SingleOrDefault(r => r.RaceId == startRaceId);
        if (startRace != null)
        {
            rows = rows
                .Where(r => r.RaceYear > startRace.Year
                    || (r.RaceYear == startRace.Year && string.Compare(r.RaceName, startRace.Name, StringComparison.Ordinal) >= 0))
                .ToList();
        }

        var endRace = DB.Races.AsNoTracking().SingleOrDefault(r => r.RaceId == endRaceId);
        if (endRace != null)
        {
            rows = rows
                .Where(r => r.RaceYear < endRace.Year
                    || (r.RaceYear == endRace.Year && string.Compare(r.RaceName, endRace.Name, StringComparison.Ordinal) <= 0))
                .ToList();
        }

        var stageAverages = rows.GroupBy(r => r.StageId).ToDictionary(g => g.Key, g => g.Average(x => x.Score));

        // Uitkomst van de query: gemiddelde relatieve score (score - gemiddelde van die stage) per speler, per stagenr.
        var perStagenrAverages = rows
            .Select(r => new { r.AccountId, r.Username, r.Stagenr, Relatief = r.Score - stageAverages[r.StageId] })
            .GroupBy(r => new { r.AccountId, r.Username, r.Stagenr })
            .Select(g => new { g.Key.AccountId, g.Key.Username, g.Key.Stagenr, Gemiddelde = g.Average(x => x.Relatief) })
            .ToList();

        // Schaling gebeurt pas na het groeperen per stagenr: het gemiddelde van de speler is het gemiddelde
        // van diens eigen per-stagenr uitkomsten hierboven, niet van de losse selecties. Genormaliseerd is
        // een shift (verschuiving) met dit gemiddelde, geen percentage-deling, zodat elke speler se lijn
        // rond 0 uitkomt.
        var spelerGemiddeldes = perStagenrAverages
            .GroupBy(x => x.AccountId)
            .ToDictionary(g => g.Key, g => g.Average(x => x.Gemiddelde));

        var uitslagen = perStagenrAverages
            .GroupBy(x => x.Stagenr)
            .OrderBy(g => g.Key)
            .Select(g => new Scores(
                g.Select(x =>
                {
                    var spelerGemiddelde = spelerGemiddeldes[x.AccountId];
                    var value = genormaliseerd ? x.Gemiddelde - spelerGemiddelde : x.Gemiddelde;
                    return new UsernameScore(x.Username, (int)Math.Round(value));
                }).ToList(),
                g.Key.ToString()))
            .ToList();

        var participants = perStagenrAverages
            .Select(x => new { x.AccountId, x.Username })
            .Distinct()
            .OrderBy(x => x.AccountId)
            .Select(x => x.Username)
            .ToList();

        var start = new Scores(participants.Select(x => new UsernameScore(x, 0)).ToList(), "");

        return new(participants, uitslagen.Prepend(start).Select(x => ConvertToDict(x)));
    }

    public LineChartData PerfectScoreVerloop(int raceId, bool budgetParticipation)
    {
        var missedPoints = MissedPoints(raceId, budgetParticipation).ToList();
        var summed = missedPoints.Select(x => x with { Data = SumMissedPointsData(x.Data) });
        var participants = GetParticipants(raceId, budgetParticipation);

        var result = new List<Scores>
        {
            new(participants.Select(x => new UsernameScore(x, 0)).ToList(), "0")
        };

        for (int i = 0; i < summed.First().Data.Count; i++)
        {
            result.Add(new Scores(summed.Select(x => new UsernameScore(x.Username, x.Data[i].Optimaal)).ToList(), (i + 1).ToString()));
        }

        return new(participants, result.Take(result.Count - 1).Select(x => ConvertToDict(x with { UsernamesAndScores = MinusAverage(x.UsernamesAndScores) })));
    }

    public List<UsernameScore> MinusAverage(IEnumerable<UsernameScore> input)
        => input.Select(x => x with { Score = x.Score - (int)input.Average(y => y.Score) }).ToList();

    public List<MissedPointsData> SumMissedPointsData(IEnumerable<MissedPointsData> input)
    {
        var output = new List<MissedPointsData>();
        for (int i = 0; i < input.Count(); i++)
        {
            output.Add(new MissedPointsData("0", 0, input.Take(i + 1).Sum(x => x.Optimaal), 0));
        }
        return output.ToList();
    }

    private IEnumerable<Scores> StandPerEtappe(int raceId, bool budgetParticipation)
    {
        var subquery = from ss in DB.StageSelections
                       where ss.Stage.RaceId == raceId && ss.AccountParticipation.BudgetParticipation == budgetParticipation && ss.Stage.Finished
                       select new
                       {
                           ss.AccountParticipation.Account.Username,
                           ss.TotalScore,
                           ss.Stage.Stagenr,
                           ss.AccountParticipation.AccountId
                       };

        var result = subquery
            .GroupBy(ss => ss.Stagenr)
            .Select(g => new Scores(
                g.OrderByDescending(ss => ss.TotalScore)
                 .Select(ss => new UsernameScore(ss.Username, ss.TotalScore))
                 .ToList(),
                $"{g.Key}"))
            .ToList();

        return result;
    }

    public LineChartData PositieVerloop(int raceId, bool budgetParticipation)
    {
        var uitslagen = StandPerEtappe(raceId, budgetParticipation);

        var participants = GetParticipants(raceId, budgetParticipation);
        var startPos = (int)(participants.Count / -2d);
        var etappeUitslagen = new List<Scores>
        {
            new(participants.Select(x => new UsernameScore(x,startPos)).ToList(), "0")
        };

        foreach (var uitslag in uitslagen)
        {
            var etappeUitslag = new Scores([], uitslag.StageNumber);
            var rank = 0;
            var userscores = uitslag.UsernamesAndScores.ToList();
            var timesTied = 0;
            for (int i = 0; i < userscores.Count; i++)
            {
                var user = userscores[i];
                if (rank == 0 || user.Score < userscores[i - 1].Score)
                {
                    rank++;
                    rank += timesTied;
                    timesTied = 0;
                }
                else timesTied++;
                etappeUitslag.UsernamesAndScores.Add(new UsernameScore(user.Username, rank * -1));
            }
            etappeUitslagen.Add(etappeUitslag);
        }
        return new(participants, etappeUitslagen.Select(x => ConvertToDict(x)));
    }

    private List<string> GetParticipants(int raceId, bool budgetParticipation)
        => DB.AccountParticipations.Include(x => x.Account)
            .Where(x => x.RaceId == raceId && x.BudgetParticipation == budgetParticipation)
            .OrderBy(x => x.AccountId)
            .Select(x => x.Account.Username).ToList();

    public IEnumerable<RaceSelection> RacePositieScoreRaceOptions()
    {
        var query = from ap in DB.AccountParticipations.Include(ap => ap.Race)
                    where ap.Race.Finished && ap.RaceId != 99 && ap.Race.Name != "classics"
                    group ap.Race by ap.Race into g
                    select g.Key;

        return query.ToList().OrderByDescending(x => x.Year).ThenByDescending(x => x.Name)
            .Select(x => new RaceSelection($"{char.ToUpper(x.Name[0]) + x.Name[1..]} \t {x.Year}", x.RaceId));
    }

    public LineChartData RacePositieScoreVerloop(bool budgetParticipation, bool bigFour, bool relative, int startRaceId, int endRaceId, PositionScoringMethod puntentelling)
    {
        var query = DB.AccountParticipations
            .Include(ap => ap.Race)
            .Include(ap => ap.Account)
            .Where(ap => ap.Race.Finished && ap.BudgetParticipation == budgetParticipation && ap.RaceId != 99 && ap.Race.Name != "classics");

        if (bigFour) query = query.Where(ap => ap.AccountId <= 5);

        var participations = query.ToList();

        var startRace = DB.Races.AsNoTracking().SingleOrDefault(r => r.RaceId == startRaceId);
        if (startRace != null)
        {
            participations = participations
                .Where(ap => ap.Race.Year > startRace.Year
                    || (ap.Race.Year == startRace.Year && string.Compare(ap.Race.Name, startRace.Name, StringComparison.Ordinal) >= 0))
                .ToList();
        }

        var endRace = DB.Races.AsNoTracking().SingleOrDefault(r => r.RaceId == endRaceId);
        if (endRace != null)
        {
            participations = participations
                .Where(ap => ap.Race.Year < endRace.Year
                    || (ap.Race.Year == endRace.Year && string.Compare(ap.Race.Name, endRace.Name, StringComparison.Ordinal) <= 0))
                .ToList();
        }

        var positionScoresByRace = participations
            .GroupBy(ap => ap.Race)
            .SelectMany(g => RankRace(g.ToList(), puntentelling).Select(x => (x.Ap.Account, Race: g.Key, x.PositionScore)))
            .ToList();

        var allRaces = positionScoresByRace
            .Select(x => x.Race)
            .DistinctBy(r => r.RaceId)
            .OrderBy(r => r.Year).ThenBy(r => r.Name)
            .ToList();

        var modifiedList = new List<(Account Account, int CumulativeScore, Race Race)>();
        foreach (var user in positionScoresByRace.GroupBy(x => x.Account))
        {
            var scoresByRaceId = user.ToDictionary(x => x.Race.RaceId, x => x.PositionScore);
            var sum = 0;
            foreach (var race in allRaces)
            {
                sum += scoresByRaceId.GetValueOrDefault(race.RaceId, 0);
                modifiedList.Add((user.Key, sum, race));
            }
        }

        var uitslagen = modifiedList
            .GroupBy(x => x.Race)
            .Select(g => new
            {
                g.Key,
                Scores = new Scores(
                    g.OrderBy(x => x.Account.AccountId)
                     .Select(x => new UsernameScore(x.Account.Username, x.CumulativeScore))
                     .ToList(),
                    $"{g.Key.Year} {char.ToUpper(g.Key.Name[0]) + g.Key.Name[1..]}")
            })
            .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Name)
            .Select(x => x.Scores)
            .ToList();

        var participants = participations
            .Select(ap => ap.Account)
            .DistinctBy(a => a.AccountId)
            .OrderBy(a => a.AccountId)
            .Select(a => a.Username)
            .ToList();

        var start = new Scores(participants.Select(x => new UsernameScore(x, 0)).ToList(), "");

        if (relative)
            return new(participants, uitslagen.Prepend(start).Select(ConvertToDictRelative));

        return new(participants, uitslagen.Prepend(start).Select(ConvertToDict));
    }

    private static Dictionary<string, object> ConvertToDictRelative(Scores scores)
    {
        var dict = new Dictionary<string, object>
        {
            {"Name", scores.StageNumber}
        };
        var average = scores.UsernamesAndScores.Count > 0 ? scores.UsernamesAndScores.Average(x => x.Score) : 0;
        foreach (var userscore in scores.UsernamesAndScores)
        {
            dict.Add(userscore.Username, userscore.Score - average);
        }
        return dict;
    }

    private static int PointsForRank(int rank, int count, PositionScoringMethod puntentelling) => puntentelling switch
    {
        PositionScoringMethod.AantalDeelnemersMinPositie => count - rank,
        PositionScoringMethod.EenPuntPerOverwinning => rank == 1 ? 1 : 0,
        PositionScoringMethod.Podium521 => rank switch { 1 => 5, 2 => 2, 3 => 1, _ => 0 },
        PositionScoringMethod.Podium421 => rank switch { 1 => 4, 2 => 2, 3 => 1, _ => 0 },
        PositionScoringMethod.Podium321 => rank switch { 1 => 3, 2 => 2, 3 => 1, _ => 0 },
        _ => throw new ArgumentOutOfRangeException(nameof(puntentelling), puntentelling, "Onbekende puntentelling methode."),
    };

    private static List<(AccountParticipation Ap, int PositionScore)> RankRace(List<AccountParticipation> raceParticipations, PositionScoringMethod puntentelling)
    {
        var ordered = raceParticipations.OrderByDescending(ap => ap.FinalScore ?? 0).ToList();
        var count = ordered.Count;
        var result = new List<(AccountParticipation, int)>();
        var rank = 0;
        var extraRankIncreaseAfterTie = 0;
        for (int i = 0; i < ordered.Count; i++)
        {
            var ap = ordered[i];
            if (rank == 0 || (ap.FinalScore ?? 0) < (ordered[i - 1].FinalScore ?? 0))
            {
                rank += 1 + extraRankIncreaseAfterTie;
                extraRankIncreaseAfterTie = 0;
            }
            else
            {
                extraRankIncreaseAfterTie++;
            }
            result.Add((ap, PointsForRank(rank, count, puntentelling)));
        }
        return result;
    }
}