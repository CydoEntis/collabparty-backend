using Microsoft.AspNetCore.Http;


namespace CollabParty.Application.Common.Errors
{
    public class AlreadyExistsException : ServiceException
    {
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();

        public AlreadyExistsException(string field, string fieldMessage)
            : base(StatusCodes.Status409Conflict, "Already Exists", "Resource already exists.")
        {
            Errors.Add(new ErrorField { Field = field, Message = fieldMessage });
        }
    }
}