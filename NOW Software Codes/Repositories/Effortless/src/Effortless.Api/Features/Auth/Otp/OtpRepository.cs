using Effortless.Core.Domain.Entities;
using Effortless.Core.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Effortless.Api.Features.Auth.Otp;

internal interface IOtpRepository
{
    Task<OtpEntity> AddOtpAsync(OtpEntity otp);
    Task<OtpEntity> DeleteOtpAsync(OtpEntity otp);
    Task<OtpEntity?> GetLastOtpByUserIdAndOtpTypeAsync(string userId, int otpType);
    Task<OtpEntity?> GetOtpByCodeAndTypeAsync(long otpCode, int otpType, string? phoneNumber);
    Task<OtpEntity?> GetOtpByTypeAndUserIdAsync(string userId, int otpType);
    Task<long> GetOtpCountByTypeAndUserIdAsync(string? userId, int? otpType);
    Task<IEnumerable<OtpEntity>> GetOtpListAsync();
    Task<IEnumerable<OtpEntity>> GetOtpListByOtpTypeAsync(int otpType);
    Task<IEnumerable<OtpEntity>> GetOtpListByUserIdAsync(string userId);
    Task<IEnumerable<OtpEntity>> GetOtpListByUserIdAndOtpTypeAsync(string userId, int otpType);
    Task<bool> UpdateOtpAsync(OtpEntity otp);
    Task DeleteOtpListAsync(OtpEntity otp);
}

internal sealed class OtpRepository : IOtpRepository
{
    private readonly EffortlessDbContext _dbContext;

    public OtpRepository(EffortlessDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OtpEntity> AddOtpAsync(OtpEntity otp)
    {
        var addedOtp = await _dbContext.Set<OtpEntity>().AddAsync(otp);

        await _dbContext.SaveChangesAsync();

        return addedOtp.Entity;
    }
    public async Task<bool> UpdateOtpAsync(OtpEntity otp)
    {
        var otpInfo = await _dbContext.Set<OtpEntity>().FindAsync(otp.Id);

        if (otpInfo is null)
        {
            return false;
        }

        _dbContext.Entry(otpInfo).CurrentValues.SetValues(otpInfo);

        await _dbContext.SaveChangesAsync();

        return true;
    }
    public async Task<OtpEntity> DeleteOtpAsync(OtpEntity otp)
    {
        var removedOtp = _dbContext.Set<OtpEntity>().Remove(otp);

        await _dbContext.SaveChangesAsync();

        return removedOtp.Entity;
    }
    public async Task DeleteOtpListAsync(OtpEntity otp)
    {
        var oldOtps = await GetOtpListByUserIdAndOtpTypeAsync(otp.UserId!,otp.Type);
        _dbContext.Set<OtpEntity>().RemoveRange(oldOtps);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<IEnumerable<OtpEntity>> GetOtpListAsync()
    {
        return await _dbContext.Set<OtpEntity>().ToListAsync();
    }
    public async Task<IEnumerable<OtpEntity>> GetOtpListByUserIdAsync(string userId)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
    public async Task<IEnumerable<OtpEntity>> GetOtpListByUserIdAndOtpTypeAsync(string userId, int otpType)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.UserId == userId && x.Type == otpType)
            .ToListAsync();
    }
    public async Task<long> GetOtpCountByTypeAndUserIdAsync(string? userId, int? otpType)
    {
        return await _dbContext.Set<OtpEntity>()
            .LongCountAsync(x => x.UserId == userId && x.Type == otpType);
    }
    public async Task<IEnumerable<OtpEntity>> GetOtpListByOtpTypeAsync(int otpType)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.Type == otpType).ToListAsync();
    }
    public async Task<OtpEntity?> GetOtpByTypeAndUserIdAsync(string userId, int otpType)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.UserId == userId && x.Type == otpType)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<OtpEntity?> GetLastOtpByUserIdAndOtpTypeAsync(string userId, int otpType)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.UserId == userId && x.Type == otpType)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<OtpEntity?> GetOtpByCodeAndTypeAsync(long otpCode, int otpType, string? phoneNumber)
    {
        return await _dbContext.Set<OtpEntity>()
            .Where(x => x.Code == otpCode && x.Type == otpType)
            .Include(x => x.User)
            .Where(x => x.User != null && x.User.PhoneNumber == phoneNumber)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
