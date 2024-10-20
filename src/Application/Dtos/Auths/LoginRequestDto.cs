using FluentValidation;

namespace Application.Dtos.Auths;

public class LoginRequestDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginUserCommandValidator : AbstractValidator<LoginRequestDto>
{
    public LoginUserCommandValidator()
    {
        RuleFor(item => item.UserName)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(16);
        RuleFor(item => item.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(16)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$");
    }
}