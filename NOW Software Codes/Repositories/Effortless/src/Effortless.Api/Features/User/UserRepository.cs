using Effortless.Core.Domain.Entities;
using Effortless.Core.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Effortless.Api.Features.User;

public interface IUserRepository
{
    Task<UserEntity?> GetByPhoneNumberAsync(string? phoneNumber);
    Task<IEnumerable<UserEntity>> GetAsync();
    Task<bool> UpdateAsync(UserEntity user);
}

internal sealed class UserRepository : IUserRepository
{
    private readonly EffortlessDbContext _dbContext;
    private readonly UserManager<UserEntity> _userManager;

    public UserRepository(
        EffortlessDbContext dbContext,
        UserManager<UserEntity> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserEntity>> GetAsync()
    {
        return await _dbContext.Set<UserEntity>().ToListAsync();
    }
    public async Task<UserEntity?> GetByPhoneNumberAsync(string? phoneNumber)
    {
        if (phoneNumber is null)
        {
            return null;
        }
        return await _dbContext.Set<UserEntity>()
            .Where(u => u.PhoneNumber == phoneNumber)
            .FirstOrDefaultAsync();
    }
    public async Task<UserEntity?> GetByEmailAsync(string? email)
    {
        if (email is null)
        {
            return null;
        }

        return await _userManager.FindByEmailAsync(email);
    }
    public async Task<bool> UpdateAsync(UserEntity? user)
    {
        if (user is null)
        {
            return false;
        }

        var existingUser = await _dbContext.Set<UserEntity>()
            .Where(u => u.Id == user.Id)
            .FirstOrDefaultAsync();

        if (existingUser is null)
        {
            return false;
        }

        await _userManager.UpdateAsync(existingUser);

        return true;
    }

}
