using SpoRE.Infrastructure.Database;
using System.Timers;
using Timer = System.Timers.Timer;
using SpoRE.Infrastructure.Scrape;

namespace SpoRE.Setup;
public class Scheduler
{
    private IServiceProvider serviceProvider;

    public Scheduler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async void RunTimer()
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var raceClient = scope.ServiceProvider.GetService<RaceClient>();
            // race done -> return

            var stage = raceClient.CurrentStage(27);
            if (stage?.Starttime is null) return;

            if (stage.Starttime > DateTime.UtcNow) { ScheduleAction((stage.Starttime ?? new DateTime()) + TimeSpan.FromMinutes(1)); return; }

            var scrape = scope.ServiceProvider.GetService<Scrape>();
            if (!stage.Finished)
            {
                var nullableFinishTime = scrape.GetFinishTime();

                if (nullableFinishTime is { } finishtime && (finishtime - DateTime.UtcNow).TotalHours > 0) { ScheduleAction(finishtime - TimeSpan.FromMinutes(55)); return; }
            }

            await scrape.StageResults(stage);
            ScheduleAction(TimeSpan.FromMinutes(1));
        }
    }

    private void ScheduleAction(DateTime targetDateTime)
        => ScheduleAction(targetDateTime - DateTime.UtcNow);

    private void ScheduleAction(TimeSpan timespan)
    {
        Timer timer = new Timer(timespan.TotalMilliseconds);
        timer.Elapsed += (object sender, ElapsedEventArgs e) => { RunTimer(); };
        timer.AutoReset = false;
        timer.Start();
    }
}
