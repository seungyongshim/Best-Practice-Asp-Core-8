using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace FastEndpoint_8.Endpoints;


    public record ResponseDto;

public class RootEndpoint : Endpoint<RequestDto, ResponseDto>
{
    public override void Configure()
    {
        Post("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RequestDto req, CancellationToken ct)
    {
        throw new Exception("here blahblahblah");

        await SendAsync(new());
    }
}


public record RequestDto
{
    [NotNull]
    public required string FullName { get; init; }
}

file class RequestDtoValidator : Validator<RequestDto>
{
    public RequestDtoValidator()
    {
        RuleFor(x => x.FullName).NotNull();
    }
}
