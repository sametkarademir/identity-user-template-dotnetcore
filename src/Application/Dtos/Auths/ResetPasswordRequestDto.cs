using FluentValidation;

namespace Application.Dtos.Auths;

public class ResetPasswordRequestDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256)
            .Matches(@"^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).MaximumLength(16)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$");;
    }
}