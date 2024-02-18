using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace WebApplicationMinimalApi8.Dto;

public record MessageDto
{

    public string Body { get; init; } = "안녕하신지";

}

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.Body).NotEmpty().NotNull();
    }
}

public class MessageDtoExmaple : IMultipleExamplesProvider<MessageDto>
{
    public IEnumerable<SwaggerExample<MessageDto>> GetExamples()
    {
        yield return SwaggerExample.Create("빈값", new MessageDto { Body = "" });
        yield return SwaggerExample.Create("null", new MessageDto { Body = null });
    }
}
