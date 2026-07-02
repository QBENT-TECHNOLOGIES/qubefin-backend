using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Persistence.Repositories;

public interface IAuthRepository
{
    Task<User?> ValidateLoginAsync(string userName, string password);
    Task CreateUserSessionAsync(UserSession userSession);
    Task<UserSession?> GetUserSessionByTokenAsync(string sessionToken);
    //Task<UserSession?> GetUserSessionByUserIdAsync(UserId userId);
    Task<User?> GetUserBySessionTokenAsync(string sessionToken);
    Task<List<string>> GetPermissionsAsync(Guid userId);
    void UpdateSession(UserSession userSession);
}

public class AuthRepository(QubeFinDataContext context) : IAuthRepository
{
    public Task CreateUserSessionAsync(UserSession userSession)
    {
        context.TblUserSessions.Add(userSession.ToEntity());
        return Task.CompletedTask;
    }

    public async Task<List<string>> GetPermissionsAsync(Guid userId)
    {
        var userEntity = await context.TblUsers.AsNoTracking()
            .Where(m => m.Id == userId).FirstOrDefaultAsync();

        if (userEntity == null)
        {
            return new List<string>();
        }
        if (userEntity.EmployeeId == null)
        {
            return new List<string>();
        }

        var employeeEntity = await context
            .TblEmployees.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == userEntity.EmployeeId);

        if (employeeEntity == null)
        {
            return await context.TblRolePermissions
            .AsNoTracking()
            .Select(m => m.Permission.Name)
            .Distinct()
            .ToListAsync();
        }

        return await context.TblRolePermissions
            .AsNoTracking()
            //.Where(m => m.RoleId == userEntity.RoleId)
            .Select(m => m.Permission.Name)
            .Distinct()
            .ToListAsync();
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

    public async Task<UserSession?> GetUserSessionByTokenAsync(string sessionToken)
    {
        var userSessionEntity = await context.TblUserSessions
            .AsNoTracking()
            .Where(m => m.SessionToken.Equals(sessionToken))
            .FirstOrDefaultAsync();

        return userSessionEntity is null ? null : userSessionEntity.ToDomain();
    }

    public void UpdateSession(UserSession userSession)
    {
        context.TblUserSessions.Update(userSession.ToEntity());
    }

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
