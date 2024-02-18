using FluentValidation;

namespace WebApplicationMinimalApi8.Dto;

public record MessageDto
{
    [SwaggerParameterExample]
    public required string Body { get; init; }

}

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
    }
}   
