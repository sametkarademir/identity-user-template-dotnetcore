using FluentValidation;

namespace Application.Dtos.Auths;

public class ConfirmEmailRequestDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequestDto>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256).Matches(@"^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$");
    }
}