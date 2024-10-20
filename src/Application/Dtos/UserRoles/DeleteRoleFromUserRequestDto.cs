using FluentValidation;

namespace Application.Dtos.UserRoles;

public class DeleteRoleFromUserRequestDto
{
    public string UserId { get; set; } = null!;
    public string RoleId { get; set; } = null!;
}

public class DeleteRoleFromUserRequestValidator : AbstractValidator<DeleteRoleFromUserRequestDto>
{
    public DeleteRoleFromUserRequestValidator()
    {
        RuleFor(item => item.UserId).NotEmpty().NotNull();
        RuleFor(item => item.RoleId).NotEmpty().NotNull();
    }
}