using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;

public class UserReadOnlyRepository : IUserReadOnlyRepository
{
    private readonly IDapperRepository _repository;

    public UserReadOnlyRepository(IDapperRepository repository)
    {
        _repository = repository;
    }

    public Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken)
    {
        const string sql =
        """
        SELECT
            "UserId",
            "Name",
            "Email",
            "PasswordHash",
            "Role",
            "IsActive",
            "CreatedAt"
        FROM "Users"
        WHERE "Email" = @pEmail;
        """;

        return _repository.QuerySingleAsync<User>(
            sql,
            new { pEmail = email },
            cancellationToken);
    }
}