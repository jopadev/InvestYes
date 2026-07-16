namespace InvestYes.Application.Features.Register;

public sealed record RegisterUserResponse(
    Guid Id,
    string Name,
    string Email);


