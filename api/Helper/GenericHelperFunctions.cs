using SpoRE.Infrastructure.Database;
using SpoRE.Models.Response;

namespace SpoRE.Helper;

public static class HelperFunctions
{
    public static TeamSelections OrderSelectedRiders(List<UserSelection> selecties)
    {
        var riders = selecties.SelectMany(selection => selection.Riders.Select(rider => (selection.Username, rider)))
            .GroupBy(x => x.rider.Rider).Select(g => g.ToList())
            .OrderByDescending(rider => rider.Count)
            .ThenByDescending(rider => rider.Max(u => u.rider.TotalScore)).ToList();

        var allSelectedRiders = riders.Select(rider => new RiderCount(rider.First().rider.Rider, rider.Count, rider.Select(r2 => r2.Username), rider.First().rider.Selected)).ToList();

        var reorderedRiders = new List<List<(string, StageComparisonRider)>>();

        while (riders.Any())
        {
            var riderLine = new List<(string, StageComparisonRider)>();
            while (riderLine.Count < selecties.Count)
            {
                var nextRider = riders.FirstOrDefault(x => !x.Select(x => x.Item1).Intersect(riderLine.Select(x => x.Item1)).Any());
                if (nextRider is null)
                {
                    foreach (var user in selecties.Select(x => x.Username).Except(riderLine.Select(x => x.Item1)))
                    {
                        riderLine.Add(new(user, new() { TotalScore = -1 }));
                    }
                    continue;
                }
                riderLine.AddRange(nextRider);
                riders.Remove(nextRider);
            }
            reorderedRiders.Add(riderLine);
        }

        return new(selecties.Select(user => UpdateUser(user, reorderedRiders)), allSelectedRiders);
    }

    public static UserSelection UpdateUser(UserSelection user, List<List<(string, StageComparisonRider)>> reorderedRiders)
    {
        var newRiders = reorderedRiders.Select(line => line.FirstOrDefault(x => x.Item1 == user.Username).Item2);
        var totals = new StageComparisonRider
        {
            TotalScore = newRiders.Where(rs => rs.TotalScore >= 0).Sum(rs => rs.TotalScore)
        };
        return new UserSelection(user.Username, newRiders.Append(totals), user.Gemist);
    }
}

public record TeamSelections(IEnumerable<UserSelection> Teams, IEnumerable<RiderCount> Counts);

public record RiderCount(Rider Rider, int Count, IEnumerable<string> Users, StageSelectedEnum Selected);
