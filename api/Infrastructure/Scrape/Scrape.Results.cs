using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{

    public void Results(int raceId, int stagenr)
    {
        var stage = DB.Stages.SingleOrDefault(s => s.Stagenr == stagenr && s.RaceId == raceId);

        HtmlWeb web = new HtmlWeb();
        var html = web.Load("https://www.procyclingstats.com/race/volta-a-catalunya/2023/stage-6").DocumentNode; // TODO variable string
        var tabs = html.QuerySelectorAll("restabs");
        var tables = html.QuerySelectorAll("result-cont table.results tbody");
        foreach (var table in tabs.Zip(tables))
        {

        }
        var pcsId = "";
        var results = DB.ResultsPoints.Include(rp => rp.RiderParticipation.Rider).Where(rp => rp.StageId == stage.StageId);
        var rider = results.SingleOrDefault(r => r.RiderParticipation.Rider.PcsId == pcsId, new ResultsPoint());
        rider.Kompos = 1;
        DB.SaveChanges();






        if (DB.Stages.Count(s => s.RaceId == raceId) == stagenr + 1)
        {
            // eindklassement
        }
    }
}
