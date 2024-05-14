namespace SpoRE.Models
{
    public class Error(string reasonPhrase) : IValidationMessage
    {
        public ValidationSeverity Severity => ValidationSeverity.Error;

        public string PropertyName => "";

        public string Message { get; set; } = reasonPhrase;
    }
}
