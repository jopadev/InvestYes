using MediatR;

namespace InvestYes.Application.Features.Register.Commands;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password)
    : IRequest<RegisterUserResponse>;


