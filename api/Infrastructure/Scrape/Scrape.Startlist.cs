using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private static string StartlistQuery(int raceId, HtmlNode html, IEnumerable<ScRider> riderQualities)
    {   // TODO (verre toekomst) kijk of dit kan zonder ExecuteSqlRaw
        var riderInserts = new List<string>();
        var riderParticipationInserts = new List<string>();
        var rpIds = new List<string>();
        foreach (var team in html.QuerySelector("ul.startlist_v4").Children())
        {
            var teamName = team.QuerySelector("a.team").InnerText;
            teamName = TeamNameAlias(teamName.Substring(0, teamName.IndexOf('(') - 1));
            foreach (var rider in team.QuerySelectorAll("li"))
            {
                // Extract PCS data
                var names = rider.QuerySelector("a").InnerText.Split(" ");
                var lastname = string.Join(" ", names.Where(n => n.ToUpperInvariant().Equals(n))).Replace("'", "''");
                var firstnames = names.Where(n => !n.ToUpperInvariant().Equals(n));
                var initials = string.Join(".", firstnames.Select(vn => vn[0]));
                var firstname = string.Join(" ", firstnames).Replace("'", "''");
                var pcsId = rider.QuerySelector("a").GetAttributeValue("href", "").Substring(6);
                var country = rider.QuerySelector("span.flag").GetAttributeValue("class", "").Split(" ")[1];
                var riderInsert = $"('{pcsId}', '{country}', '{firstname}', '{lastname}', '{initials}')";

                // Combine with price/qualities data
                var riderId = $"(SELECT rider_id FROM rider WHERE PCS_id = '{pcsId}')";
                var sclist = riderQualities.Where(rq => CompareName(rq.FirstName, rq.LastName, firstname, lastname));
                if (sclist.Count() == 1)
                {
                    var sc = sclist.First();
                    var q = sc.Qualities.ToDictionary(q => q.Type, q => q.Value);
                    var type = MapType(sc.Type);

                    riderParticipationInserts.Add($"({raceId}, {riderId}, {(int)sc.Price}, '{teamName}', {Qval(0, q)}, {Qval(1, q)}, {Qval(2, q)}, {Qval(3, q)}, {Qval(4, q)}, '{type}')");
                    riderInserts.Add(riderInsert);
                    rpIds.Add($"(SELECT rider_participation_id FROM rider_participation WHERE rider_id = {riderId} AND race_id = {raceId})");
                }
                else if (sclist.Count() > 1)
                {
                    Console.WriteLine($"Multiple matches for rider: {string.Join(" ", names)}");
                }
                else
                {
                    Console.WriteLine($"No price data for rider: {string.Join(" ", names)}");
                }
            }
        }
        return BuildQuery(riderInserts, riderParticipationInserts, rpIds, raceId);
    }

    private static RiderType MapType(int type)
        => type switch
        {
            1 => RiderType.Klassement,
            2 => RiderType.Klimmer,
            3 => RiderType.Tijdrijder,
            4 => RiderType.Sprinter,
            5 => RiderType.Aanvaller,
            _ => RiderType.Knecht
        };

    private static int Qval(int pos, Dictionary<int, int> dict)
    {
        if (!dict.TryGetValue(pos, out int value))
        {
            value = 0;
        }
        return value;
    }

    private static string BuildQuery(IEnumerable<string> riderInserts, IEnumerable<string> riderParticipationInserts, IEnumerable<string> rpIds, int raceId)
    {
        var riderQuery = "INSERT INTO rider(pcs_id, country, firstname, lastname, initials) VALUES" + string.Join(", ", riderInserts) + " ON CONFLICT (PCS_id) DO UPDATE SET country = EXCLUDED.country, firstname = EXCLUDED.firstname, lastname = EXCLUDED.lastname, initials = EXCLUDED.initials;\n ";
        var participationQuery = "INSERT INTO rider_participation (race_id, rider_id, price, team, gc, climb, tt, sprint, punch, type) VALUES" + string.Join(", ", riderParticipationInserts) + " ON CONFLICT (race_id,rider_id) DO UPDATE SET team = EXCLUDED.team, price = EXCLUDED.price, gc = EXCLUDED.gc, climb = EXCLUDED.climb, tt = EXCLUDED.tt, sprint = EXCLUDED.sprint, punch = EXCLUDED.punch, type = EXCLUDED.type;\n ";

        var startListIds = $"({string.Join(", ", rpIds)})";
        var ridersInRace = $"(SELECT rider_participation_id FROM rider_participation WHERE race_id = {raceId})";
        var deleteStageSelectionQuery = $"DELETE FROM stage_selection_rider WHERE rider_participation_id NOT IN {startListIds} AND rider_participation_id IN {ridersInRace};\n ";
        var deleteKopmanQuery = $"UPDATE stage_selection SET kopman_id = NULL WHERE kopman_id NOT IN {startListIds} AND kopman_id IN {ridersInRace};\n ";
        var deleteTeamSelectionQuery = $"DELETE FROM team_selection_rider WHERE rider_participation_id NOT IN {startListIds} AND rider_participation_id IN {ridersInRace};\n ";
        var deleteStartlistQuery = $"DELETE FROM rider_participation WHERE rider_participation_id NOT IN {startListIds} AND race_id = {raceId};\n ";

        return deleteStageSelectionQuery + deleteKopmanQuery + deleteTeamSelectionQuery + deleteStartlistQuery + riderQuery + participationQuery;
    }

    private static bool CompareName(string firstnameSC, string lastnameSC, string firstname, string lastname)
        => firstname.Contains(firstnameSC, StringComparison.InvariantCultureIgnoreCase)
            && lastname.Contains(lastnameSC, StringComparison.InvariantCultureIgnoreCase);
}

internal class PrijzenFile
{
    public IEnumerable<ScRider> Content { get; set; }
}

public record ScRider(
    int Age,
    double Price,
    int Status,
    int Type,
    string FirstName,
    string LastName,
    string NameShort,
    List<ScQuality> Qualities);

public record ScQuality(int Type, int Value);