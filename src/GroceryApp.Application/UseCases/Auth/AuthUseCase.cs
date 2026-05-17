using GroceryApp.Application.DTOs;
using GroceryApp.Application.Interfaces;
using GroceryApp.Domain.Entities;
using GroceryApp.Domain.Repositories;

namespace GroceryApp.Application.UseCases.Auth;

public class AuthUseCase(
    IUserRepository userRepo,
    IJwtService jwtService,
    IPasswordService passwordService)
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepo.GetByUsernameAsync(request.Username);
        if (user == null || !passwordService.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("ユーザー名またはパスワードが正しくありません");
        return new AuthResponse(jwtService.GenerateToken(user), user.Username);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string validCode)
    {
        if (string.IsNullOrEmpty(validCode) || request.RegisterCode != validCode)
            throw new UnauthorizedAccessException("登録コードが正しくありません");

        if (await userRepo.GetByUsernameAsync(request.Username) != null)
            throw new InvalidOperationException("そのユーザー名はすでに使用されています");

        var user = User.Create(request.Username, passwordService.Hash(request.Password));
        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();
        return new AuthResponse(jwtService.GenerateToken(user), user.Username);
    }
}
