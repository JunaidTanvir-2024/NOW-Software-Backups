using Effortless.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace Effortless.Core.Services.Identity;

internal sealed class AuthService(
    SignInManager<UserEntity> signInManager,
    UserManager<UserEntity> userManager) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<SignInResult?> LoginAsync(UserEntity? user, string password, bool rememberMe = false, bool lockoutOnFailure = false)
    {
        ArgumentNullException.ThrowIfNull(user);
        return await _signInManager.PasswordSignInAsync(user.UserName!, password, rememberMe, lockoutOnFailure: lockoutOnFailure);
    }
    public async Task<IdentityResult?> RegisterAsync(UserEntity? user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        return await _userManager.CreateAsync(user, password);
    }
    public async Task<IdentityResult> PasswordResetAsync(UserEntity? user, string newPassword)
    {
        ArgumentNullException.ThrowIfNull(user);

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (!removePasswordResult.Succeeded)
        {
            return removePasswordResult;
        }

        return await _userManager.AddPasswordAsync(user, newPassword);
    }
}
