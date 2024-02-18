using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace WebApplicationMinimalApi8.Dto;

public record MessageDto
{

    public string NullString { get; init; } = "기본값";
    public IEnumerable<string> NullStrings { get; init; } = [];

}

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.NullString).NotEmpty();
    }
}

#pragma warning disable CS8625

public class MessageDtoExmaple : IMultipleExamplesProvider<MessageDto>
{
    public IEnumerable<SwaggerExample<MessageDto>> GetExamples()
    {
        yield return SwaggerExample.Create("null", new MessageDto
        {
            NullString = null,
            NullStrings = null
        });
        yield return SwaggerExample.Create("notnull", new MessageDto { NullString = "값", NullStrings = ["값"] });
        yield return SwaggerExample.Create("error", new MessageDto { NullString = "" });
    }
}
