using FluentValidation;

namespace WebApplicationMinimalApi8.Dto;

public record MessageDto
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>안녕하세요</example>
    public required string Body { get; init; }

}

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
    }
}   
