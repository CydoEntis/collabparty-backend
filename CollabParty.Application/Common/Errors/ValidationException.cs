using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;


namespace CollabParty.Application.Common.Errors
{
    public class ValidationException : ServiceException
    {
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();

        public ValidationException(string field, string fieldMessage)
            : base(StatusCodes.Status400BadRequest, ErrorTitles.ValidationException, ErrorMessages.ValidationFailed)
        {
            Errors.Add(new ErrorField { Field = field, Message = fieldMessage });
        }
    }
}