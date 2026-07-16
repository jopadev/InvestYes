using InvestYes.Application.Features.Register.Commands;
using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterUserResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var exists =
            await _repository.ExistsByEmailAsync(
                request.Email,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "Já existe um usuário cadastrado com este e-mail.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Investor",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
     user,
     cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new RegisterUserResponse(
            user.UserId,
            user.Name,
            user.Email);
    }
}

