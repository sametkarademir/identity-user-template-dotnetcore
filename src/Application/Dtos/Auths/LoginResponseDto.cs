namespace Application.Dtos.Auths;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime DateExpiresUtc { get; set; }
}