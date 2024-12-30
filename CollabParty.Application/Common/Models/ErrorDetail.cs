using System.Collections.Generic;

namespace CollabParty.Application.Common.Models
{
    public class ErrorDetail
    {
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string>? Errors { get; set; } = new Dictionary<string, string>();

        public ErrorDetail(string title, int statusCode, Dictionary<string, string> errors)
        {
            Title = title;
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}