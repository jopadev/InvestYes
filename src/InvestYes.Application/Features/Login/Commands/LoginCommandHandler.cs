using InvestYes.Application.Interfaces;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Login.Commands;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserReadOnlyRepository _userReadRepository;
    public LoginCommandHandler(IUserRepository userRepository,IJwtTokenGenerator jwtTokenGenerator, IUserReadOnlyRepository userReadRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userReadRepository = userReadRepository;
    }

    public async Task<LoginResponse> Handle(LoginCommand request,CancellationToken cancellationToken)
    {
        var oUser = await _userReadRepository.GetByEmailAsync(request.Email,cancellationToken);

        if (oUser is null)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var bValidPassword = BCrypt.Net.BCrypt.Verify(request.Password,oUser.PasswordHash);

        if (!bValidPassword)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        if (!oUser.IsActive)
            throw new UnauthorizedAccessException("Usuário inativo.");

        var sToken = _jwtTokenGenerator.Generate(oUser);

        return new LoginResponse(sToken,DateTime.UtcNow.AddHours(2),oUser.UserId,oUser.Name,oUser.Email,oUser.Role);
    }
}