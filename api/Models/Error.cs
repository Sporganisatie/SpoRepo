namespace SpoRE.Models
{
    public class Error : IValidationMessage
    {
        public Error(string reasonPhrase)
        {
            Message = reasonPhrase;
        }

        public ValidationSeverity Severity => ValidationSeverity.Error;

        public string PropertyName => "";

        public string Message { get; set; }
    }
}
