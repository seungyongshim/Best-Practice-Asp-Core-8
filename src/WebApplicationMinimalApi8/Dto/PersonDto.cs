namespace WebApplicationMinimalApi8.Dto;

public record PersonDto
{
    public required string Name { get; init; }
    public required int Age { get; init; }
    public IEnumerable<string> Emails { get; init; } = [];
}
