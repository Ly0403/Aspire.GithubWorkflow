namespace Shared.Abstracts;

public sealed record HttpCustomResponse<T>
{
    public string Title { get; init; } = string.Empty;
    public string? Details { get; init; } = string.Empty;
    public int Status { get; init; }
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public Guid InstanceId => Guid.CreateVersion7();
    public string Type => "https://tools.ietf.org/html/rfc7231#section-6.5.1";
}