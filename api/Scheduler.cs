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
            var DB = scope.ServiceProvider.GetService<DatabaseContext>();
            // race done -> return

            var stage = DB.CurrentStage(29);
            if (stage?.Starttime is null) return;

            if (stage.Starttime > DateTime.UtcNow) { ScheduleAction(TimeSpan.FromMinutes(1)); return; }

            var scrape = scope.ServiceProvider.GetService<Scrape>();
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
