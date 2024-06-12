using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class RiderService(DatabaseContext DB)
{
    internal RiderInfo GetRiderInfo(int riderId)
    {
        var b = from rider in DB.Riders
                join rp in DB.RiderParticipations.Include(rp => rp.Race).Where(rp => rp.Race.Name != "classics") on rider.RiderId equals rp.RiderId into participations
                where rider.RiderId == riderId
                select new RiderInfo(rider, participations);
        // select resultaten van recentste race
        return b.Single();
    }
}

public record RiderInfo(Rider Rider, IEnumerable<RiderParticipation> RiderParticipations);