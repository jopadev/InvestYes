using BCrypt.Net;
using MediatR;

namespace InvestYes.Application.Features.Login.Commands;
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponse>;
