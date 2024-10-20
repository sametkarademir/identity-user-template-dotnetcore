using FluentValidation;

namespace Application.Dtos.Users;

public class ChangePasswordAppUserRequestDto
{
    public string Password { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangePasswordAppUserRequestValidator : AbstractValidator<ChangePasswordAppUserRequestDto>
{
    public ChangePasswordAppUserRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(16)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).MaximumLength(16)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$");
    }
}