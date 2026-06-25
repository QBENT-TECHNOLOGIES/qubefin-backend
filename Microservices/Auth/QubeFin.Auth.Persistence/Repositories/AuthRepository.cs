using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Persistence.Repositories;

public interface IAuthRepository
{
    Task<User?> ValidateLoginAsync(string userName, string password);
}

public class AuthRepository(QubeFinDataContext context) : IAuthRepository
{
    public async Task<User?> ValidateLoginAsync(string userName, string password)
    {
        var userEntity = await context.TblUsers.AsNoTracking()
            .Where(m => m.UserName == userName).FirstOrDefaultAsync();

        if (userEntity is null)
        {
            return null;
        }

        var appUser = new AppUser(userName, password);
        var passwordHasher = new PasswordHasher<AppUser>();
        var result = passwordHasher.VerifyHashedPassword(appUser, userEntity.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return userEntity.ToDomain();
    }
}
