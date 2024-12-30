namespace CollabParty.Application.Common.Errors
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public List<Dictionary<string, string>> ErrorField { get; set; } = new List<Dictionary<string, string>>();

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        // Add metadata to the error
        public void AddErrorField(string field, string message)
        {
            var metadata = new Dictionary<string, string>
            {
                { field, message }
            };
            ErrorField.Add(metadata);
        }

        // Alternatively, add a method to add multiple metadata entries at once
        public void AddErrorField(Dictionary<string, string> errorField)
        {
            ErrorField.Add(errorField);
        }
    }
}