using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    DatabaseContext DB;
    public Scrape(DatabaseContext databaseContext)
    {
        DB = databaseContext;
    }

    public void Startlist(string raceName, int year)
    {
        var raceId = DB.Races.Single(r => r.Name == raceName && r.Year == year).RaceId;
        var html = new HtmlWeb().Load($"https://www.procyclingstats.com/race/{RaceString(raceName)}/{year}/startlist").DocumentNode;
        var riderQualities = JsonSerializer.Deserialize<PrijzenFile>(File.ReadAllText($"./api/Infrastructure/Scrape/{Filename(raceName)}.txt")).Content; // TODO remove hardcoded
        var query = StartlistQuery(raceId, html, riderQualities);
        DB.Database.ExecuteSqlRaw(query);
    }

    private string RaceString(string raceName)
        => raceName switch
        {
            "giro" => "giro-d-italia",
            "tour" => "tour-de-france",
            "vuelta" => "vuelta-a-espana",
            _ => throw new ArgumentOutOfRangeException()
        };

    private string Filename(string raceName)
        => raceName switch
        {
            "giro" => "Giroprijzen",
            // "tour" => "Filename",
            // "vuelta" => "Filename",
            _ => throw new ArgumentOutOfRangeException()
        };
}