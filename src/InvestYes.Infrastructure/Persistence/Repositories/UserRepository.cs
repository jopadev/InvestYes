using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestYes.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly InvestYesContext _context;

    public UserRepository(InvestYesContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(string email,CancellationToken cancellationToken)
    {
        return await _context.Users
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(User user,CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }



}
