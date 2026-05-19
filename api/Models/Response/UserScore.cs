namespace SpoRE.Models.Response;

public record UserScore(string username, int stagescore, int totalscore, int change, bool isLoggedInUser);