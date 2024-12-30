using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CollabParty.Application.Common.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string, string>? Errors { get; set; }

        public ApiResponse(bool success, T? data, string? title, int statusCode, Dictionary<string, string>? errors = null)
        {
            Success = success;
            Data = data;
            Title = title;
            StatusCode = statusCode;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string title = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>(true, data, title, statusCode);
        }

        public static ApiResponse<T> ErrorResponse(string title, int statusCode, Dictionary<string, string>? errors = null)
        {
            return new ApiResponse<T>(false, default, title, statusCode, errors);
        }
    }
}