using GroceryApp.Domain.Entities;

namespace GroceryApp.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
