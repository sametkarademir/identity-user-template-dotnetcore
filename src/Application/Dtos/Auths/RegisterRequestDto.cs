using FluentValidation;

namespace Application.Dtos.Auths;

public class RegisterRequestDto
{
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .Matches(@"^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$");
        RuleFor(item => item.UserName)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(16);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(16)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$");
    }
}