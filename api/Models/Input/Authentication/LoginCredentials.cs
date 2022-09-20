using System.ComponentModel.DataAnnotations;

namespace SpoRE.Models.Input.Authentication;

// als een van deze 2 ontbreekt is er meteen een bad request response met info over de fout test maar met swagger https://localhost:7072/swagger
public record LoginCredentials([Required] string Email, [Required] string Password);