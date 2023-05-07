using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record AccountStageResults(Account account, int stagescore, int totalscore);