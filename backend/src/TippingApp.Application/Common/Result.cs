namespace TippingApp.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public bool IsNotFound { get; }

    private Result(bool isSuccess, T? value, string? error, bool isNotFound)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        IsNotFound = isNotFound;
    }

    public static Result<T> Success(T value) => new(true, value, null, false);
    public static Result<T> NotFound(string error = "Not found") => new(false, default, error, true);
    public static Result<T> Failure(string error) => new(false, default, error, false);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public bool IsNotFound { get; }

    private Result(bool isSuccess, string? error, bool isNotFound)
    {
        IsSuccess = isSuccess;
        Error = error;
        IsNotFound = isNotFound;
    }

    public static Result Success() => new(true, null, false);
    public static Result NotFound(string error = "Not found") => new(false, error, true);
    public static Result Failure(string error) => new(false, error, false);
}
