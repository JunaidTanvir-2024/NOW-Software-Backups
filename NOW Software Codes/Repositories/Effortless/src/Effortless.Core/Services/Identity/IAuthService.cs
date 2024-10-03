using Effortless.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace Effortless.Core.Services.Identity;
public interface IAuthService
{
    Task<SignInResult?> LoginAsync(UserEntity? user, string password, bool rememberMe = false, bool lockoutOnFailure = false);
    Task<IdentityResult> PasswordResetAsync(UserEntity? user, string newPassword);
    Task<IdentityResult?> RegisterAsync(UserEntity? user, string password);
}
