namespace SpoRE.Models.Input.Authentication;

public record SignupCredentials(string Email, string Password, string RepeatedPassword);