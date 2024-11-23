namespace CollabParty.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; private set; }
    public List<ValidationError> Errors { get; private set; }

    private Result(bool isSuccess, List<ValidationError> errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<ValidationError>();
    }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Failure(List<ValidationError> errors)
    {
        return new Result(false, errors);
    }
}

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public List<ValidationError> Errors { get; private set; }

    private Result(bool isSuccess, T data = default, List<ValidationError> errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors ?? new List<ValidationError>();
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data);
    }

    public static Result<T> Failure(string field, IEnumerable<string> messages)
    {
        return new Result<T>(false, default, new List<ValidationError>() { new ValidationError(field, messages) });
    }
    
    public static Result<T> Failure(List<ValidationError> errors)
    {
        return new Result<T>(false, default, errors);
    }
}