using SpoRE.Infrastructure.Database;
using System.Timers;
using Timer = System.Timers.Timer;
using SpoRE.Infrastructure.Scrape;

namespace SpoRE.Setup;

public class Scheduler(IServiceProvider ServiceProvider)
{
    public async void RunTimer()
    {
        using var scope = ServiceProvider.CreateScope();
        var DB = scope.ServiceProvider.GetService<DatabaseContext>();
        // race done -> return

        var stage = DB.CurrentStage();
        if (stage?.Starttime is null) return;

        if (stage.Starttime > DateTime.UtcNow)
        {
            ScheduleAction(TimeSpan.FromMinutes(1)); return;
            // if done schedule tomorrow
            // anders kijk op pcs hoelang de etappe nog duurt
        }

        var scrape = scope.ServiceProvider.GetService<Scrape>();
        await scrape.StageResults(stage);
        ScheduleAction(TimeSpan.FromMinutes(1));
    }

    private void ScheduleAction(DateTime targetDateTime)
        => ScheduleAction(targetDateTime - DateTime.UtcNow);

    private void ScheduleAction(TimeSpan timespan)
    {
        var timer = new Timer(timespan.TotalMilliseconds);
        timer.Elapsed += (object sender, ElapsedEventArgs e) => { RunTimer(); };
        timer.AutoReset = false;
        timer.Start();
    }
}
