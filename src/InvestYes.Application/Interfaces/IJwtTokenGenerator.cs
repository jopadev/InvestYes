using InvestYes.Domain.Entities;

namespace InvestYes.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}


