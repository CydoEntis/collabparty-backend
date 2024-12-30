using Microsoft.AspNetCore.Http;


namespace CollabParty.Application.Common.Errors
{
    public class DuplicateException : ServiceException
    {
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();

        public DuplicateException(string field, string fieldMessage)
            : base(StatusCodes.Status409Conflict, "Duplicate Error", "Input data is already in use.")
        {
            Errors.Add(new ErrorField { Field = field, Message = fieldMessage });
        }
    }
}