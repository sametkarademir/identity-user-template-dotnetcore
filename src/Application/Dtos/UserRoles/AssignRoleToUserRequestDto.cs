using FluentValidation;

namespace Application.Dtos.UserRoles;

public class AssignRoleToUserRequestDto
{
    public string UserId { get; set; } = null!;
    public string RoleId { get; set; } = null!;
}

public class AssignRoleToUserRequestValidator : AbstractValidator<AssignRoleToUserRequestDto>
{
    public AssignRoleToUserRequestValidator()
    {
        RuleFor(item => item.UserId).NotEmpty().NotNull();
        RuleFor(item => item.RoleId).NotEmpty().NotNull();
    }
}