using InvestYes.Domain.Entities;

namespace InvestYes.Domain.Interfaces.Repositories;

public interface IUserReadOnlyRepository
{
   Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

}
