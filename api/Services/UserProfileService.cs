using Microsoft.EntityFrameworkCore;
using SpoRE.Infrastructure.Database;

namespace SpoRE.Services;

public class UserProfileService(DatabaseContext DB)
{
    public UserProfileData GetProfile(string username, bool budgetParticipation)
    {
        var target = DB.Accounts.SingleOrDefault(a => a.Username == username);

        if (target is null)
        {
            return null;
        }

        var participations = DB.AccountParticipations
            .Include(ap => ap.Race)
            .Where(ap => ap.AccountId == target.AccountId && ap.BudgetParticipation == budgetParticipation)
            .ToList();

        var overview = participations
            .Select(ap => ap.Race)
            .OrderByDescending(race => race.Year)
            .ThenByDescending(race => race.Name)
            .ToList();

        var activeRace = DB.Races
            .Where(r => !r.Finished)
            .ToList()
            .Where(r => DB.ShowResults(r.RaceId, 1))
            .OrderByDescending(r => r.RaceId)
            .FirstOrDefault();

        return new UserProfileData(
            target.Username,
            activeRace,
            overview);
    }
}

public record UserProfileData(
    string Username,
    Race CurrentRace,
    List<Race> Overview
);
