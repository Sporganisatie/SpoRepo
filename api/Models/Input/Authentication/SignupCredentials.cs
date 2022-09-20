using System.ComponentModel.DataAnnotations;

namespace SpoRE.Models.Input.Authentication;

public record SignupCredentials([Required] string Email, [Required] string Password, [Required] string RepeatedPassword);