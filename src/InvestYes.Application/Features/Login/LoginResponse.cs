namespace InvestYes.Application.Features.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Name,
    string Email,
    string Role);


