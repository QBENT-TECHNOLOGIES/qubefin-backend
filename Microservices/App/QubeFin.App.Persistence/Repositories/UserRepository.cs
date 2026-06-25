using Microsoft.AspNetCore.Identity;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Persistence.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Task<string> HashPasswordAsync(string userName, string password);
    Task<bool> VerifyPasswordAsync(AppUser user, string password);
}

public class UserRepository(QubeFinDataContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.TblUsers.Add(user.ToEntity());
    }

    public Task<string> HashPasswordAsync(string userName, string password)
    {
        var passwordHasher = new PasswordHasher<AppUser>();
        var hashedPassword = passwordHasher.HashPassword(new AppUser(userName, password), password);
        return Task.FromResult(hashedPassword);
    }

    public Task<bool> VerifyPasswordAsync(AppUser user, string password)
    {
        var passwordHasher = new PasswordHasher<AppUser>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Failed ? Task.FromResult(false) : Task.FromResult(true);
    }
}
