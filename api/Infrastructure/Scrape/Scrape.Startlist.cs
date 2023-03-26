using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private static string StartlistQuery(int raceId, HtmlNode html, IEnumerable<ScQualities> riderQualities)
    {   // TODO (verre toekomst) kijk of dit kan zonder ExecuteSqlRaw
        var riderInserts = new List<string>();
        var riderParticipationInserts = new List<string>();
        var rpIds = new List<string>();

        foreach (var team in html.QuerySelectorAll("li.team"))
        {
            var teamName = team.QuerySelector("b a").InnerText;
            foreach (var rider in team.QuerySelectorAll("li"))
            {
                // Extract PCS data
                var names = rider.QuerySelector("a").InnerText.Split(" ");
                var lastname = String.Join(" ", names.Where(n => n.ToUpperInvariant().Equals(n)));
                var firstnames = names.Where(n => !n.ToUpperInvariant().Equals(n));
                var initials = String.Join(".", firstnames.Select(vn => vn[0]));
                var firstname = String.Join(" ", firstnames);
                var pcsId = rider.QuerySelector("a").GetAttributeValue("href", "").Substring(6);
                var country = rider.QuerySelector("span").GetAttributeValue("class", "").Split(" ")[1];
                var riderInsert = $"('{pcsId}', '{country}', '{firstname}', '{lastname}', '{initials}')";

                // Combine with price/qualities data
                var riderId = $"(SELECT rider_id FROM rider WHERE PCS_id = '{pcsId}')";
                try
                {
                    var sc = riderQualities.Single(rq => CompareName(rq.Name, names));
                    var q = sc.Qualities.ToDictionary(q => q.Name, q => q.Value);
                    riderParticipationInserts.Add($"({raceId}, {riderId}, {sc.Price}, '{teamName}', {q["Klassement"]}, {q["Klimmen"]}, {q["Tijdrijden"]}, {q["Sprinten"]}, {q["Punchen"]})");
                    riderInserts.Add(riderInsert);
                    rpIds.Add($"(SELECT rider_participation_id FROM rider_participation WHERE rider_id = {riderId} AND race_id = {raceId})");
                }
                catch (System.Exception)
                {
                    Console.WriteLine($"No price data for rider: {String.Join(" ", names)}");
                }
            }
        }
        return BuildQuery(riderInserts, riderParticipationInserts, rpIds, raceId);
    }

    private static string BuildQuery(IEnumerable<string> riderInserts, IEnumerable<string> riderParticipationInserts, IEnumerable<string> rpIds, int raceId)
    {
        var riderQuery = "INSERT INTO rider(pcs_id, country, firstname, lastname, initials) VALUES" + String.Join(", ", riderInserts) + " ON CONFLICT (PCS_id) DO UPDATE SET country = EXCLUDED.country, firstname = EXCLUDED.firstname, lastname = EXCLUDED.lastname, initials = EXCLUDED.initials;\n ";
        var participationQuery = "INSERT INTO rider_participation (race_id, rider_id, price, team, gc, climb, tt, sprint, punch) VALUES" + String.Join(", ", riderParticipationInserts) + " ON CONFLICT (race_id,rider_id) DO UPDATE SET team = EXCLUDED.team, price = EXCLUDED.price, gc = EXCLUDED.gc, climb = EXCLUDED.climb, tt = EXCLUDED.tt, sprint = EXCLUDED.sprint, punch = EXCLUDED.punch;\n ";

        var startListIds = $"({String.Join(", ", rpIds)})";
        var ridersInRace = $"(SELECT rider_participation_id FROM rider_participation WHERE race_id = {raceId})";
        var deleteStageSelectionQuery = $"DELETE FROM stage_selection_rider WHERE rider_participation_id NOT IN {startListIds} AND rider_participation_id IN {ridersInRace};\n ";
        var deleteKopmanQuery = $"UPDATE stage_selection SET kopman_id = NULL WHERE kopman_id NOT IN {startListIds} AND kopman_id IN {ridersInRace};\n ";
        var deleteTeamSelectionQuery = $"DELETE FROM team_selection_rider WHERE rider_participation_id NOT IN {startListIds} AND rider_participation_id IN {ridersInRace};\n ";
        var deleteStartlistQuery = $"DELETE FROM rider_participation WHERE rider_participation_id NOT IN {startListIds} AND race_id = {raceId};\n ";

        return deleteStageSelectionQuery + deleteKopmanQuery + deleteTeamSelectionQuery + deleteStartlistQuery + riderQuery + participationQuery;
    }

    private static bool CompareName(ScName name, string[] names)
    {
        var lastname = String.Join(" ", names.Where(n => n.ToUpper().Equals(n))).ToLowerInvariant();
        var firstname = String.Join(" ", names.Where(n => !n.ToUpper().Equals(n))).ToLowerInvariant();
        return firstname.Contains(name.FirstName.ToLowerInvariant()) && lastname.Equals(name.LastName.ToLowerInvariant());
    }
}

internal class PrijzenFile
{
    public IEnumerable<ScQualities> Content { get; set; }
}

internal class ScQualities
{
    public ScName Name { get; set; }
    public IEnumerable<Quality> Qualities { get; set; }
    public int Price { get; set; }
}

internal record Quality(string Name, int Value);

internal record ScName(string FirstName, string LastName, string FullName, string NameShort);