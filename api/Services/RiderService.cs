using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class RiderService(DatabaseContext DB)
{
    internal Rider GetRiderInfo(int riderId)
    {
        // geef een lijst van alle races terug met totalen en prijs 
        // elke race bevat prijs, positie eind klassementen en per etappe
        // punten (totalen)

        return DB.Riders
            .Include(x => x.RiderParticipations)
            .ThenInclude(x => x.Race)
            .Single(x => x.RiderId == riderId);
        // var b = from rider in DB.Riders
        //         join rp in DB.RiderParticipations.Include(rp => rp.Race).Where(rp => rp.Race.Name != "classics") on rider.RiderId equals rp.RiderId into participations
        //         where rider.RiderId == riderId
        //         select new RiderInfo(rider, participations);
        // select resultaten van recentste race
        // return b.Single();
    }
}
