using Microsoft.AspNetCore.Identity;

namespace Effortless.Core.Domain.Entities;

public class UserEntity : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset DeleteDate { get; set; }

    public virtual ICollection<OtpEntity> Otps { get; } = new List<OtpEntity>();
}
