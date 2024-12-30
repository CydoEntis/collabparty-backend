using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;


namespace CollabParty.Application.Common.Errors
{
    public class AlreadyExistsException : ServiceException
    {
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();

        public AlreadyExistsException(string field, string fieldMessage)
            : base(StatusCodes.Status409Conflict, ErrorTitles.AlreadyExists, ErrorMessages.AlreadyExists)
        {
            Errors.Add(new ErrorField { Field = field, Message = fieldMessage });
        }
    }
}