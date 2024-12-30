using Microsoft.AspNetCore.Http;


namespace CollabParty.Application.Common.Errors
{
    public class ValidationException : ServiceException
    {
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();

        public ValidationException(string field, string fieldMessage)
            : base(StatusCodes.Status400BadRequest, "Validation Exception", "One or more validation errors occurred.")
        {
            Errors.Add(new ErrorField { Field = field, Message = fieldMessage });
        }
    }
}