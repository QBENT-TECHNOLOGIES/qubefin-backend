using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Persistence.Repositories;

public interface IUserRepository
{
    void Add(User user);
    void Update(User user);
    Task<string> HashPasswordAsync(string userName, string password);
    Task<bool> VerifyPasswordAsync(AppUser user, string password);
    Task<User?> GetUserBySessionTokenAsync(string sessionToken);
    Task<bool> GetExsitingUserByUserName(Guid? id, string userName);
    Task<User> GetExistingUser(Guid? id, CancellationToken cancellation);
}

public class UserRepository(QubeFinDataContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.TblUsers.Add(user.ToEntity());
    }

    public async Task<User?> GetUserBySessionTokenAsync(string sessionToken)
    {
        var userSessionEntity = await context
            .TblUserSessions
            .AsNoTracking()
            .Where(m => m.SessionToken.Equals(sessionToken))
            .FirstOrDefaultAsync();

        if (userSessionEntity is null)
        {
            return null;
        }

        var userEntity = await context
            .TblUsers
            .Include(m => m.Employee)
            .AsNoTracking()
            .Where(m => m.Id == userSessionEntity.UserId)
            .FirstOrDefaultAsync();

        if (userEntity is null)
        {
            return null;
        }

        return userEntity.ToDomain();
    }

    public Task<string> HashPasswordAsync(string userName, string password)
    {
        var passwordHasher = new PasswordHasher<AppUser>();
        var hashedPassword = passwordHasher.HashPassword(new AppUser(userName, password), password);
        return Task.FromResult(hashedPassword);
    }

    public void Update(User user)
    {
        context.TblUsers.Update(user.ToEntity());
    }

    public Task<bool> VerifyPasswordAsync(AppUser user, string password)
    {
        var passwordHasher = new PasswordHasher<AppUser>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Failed ? Task.FromResult(false) : Task.FromResult(true);
    }
    public async Task<bool> GetExsitingUserByUserName(Guid? id, string userName)
    {

        var normalizedUserName = userName.Trim();

        return await context.TblUsers
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserName.Trim() == normalizedUserName &&
                (!id.HasValue || id.Value == Guid.Empty || x.Id != id.Value));
    }

    public async Task<User> GetExistingUser(Guid? id, CancellationToken cancellation)
    {
        var entity = await context.TblUsers.AsNoTracking().Include(m => m.Employee).Where(m => m.Id == id).FirstOrDefaultAsync(cancellation) ?? throw new Exception($"User not found for the given Id");
        return entity.ToDomain();
    }
}
