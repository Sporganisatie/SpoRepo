using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record UserScore(Account account, int stagescore, int totalscore);