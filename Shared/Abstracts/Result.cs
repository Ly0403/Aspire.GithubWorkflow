namespace Shared.Abstracts;

public sealed class Result<T>
{
    public Result(T data)
    {
        Data = data;
        IsSuccess = true;
    }

    public Result(string error)
    {
        IsSuccess = false;
        ErrorMessage = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T? Data { get; }

    public string? ErrorMessage { get; }
}
