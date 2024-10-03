using Effortless.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Effortless.Core.Persistence;

public class EffortlessDbContext : IdentityDbContext<UserEntity>
{
    public EffortlessDbContext(DbContextOptions<EffortlessDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Identity Tables Configuration
        IdentityConfiguration(modelBuilder);
        OtpTableConfiguration(modelBuilder);
    }

    private static void OtpTableConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OtpEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Otp");
            entity.Property(e => e.Code).HasColumnName("Otp");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Otps)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserId_Otp");
        });
    }

    private static void IdentityConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(b => b.ToTable("Users"));
        modelBuilder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaims"));
        modelBuilder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogins"));
        modelBuilder.Entity<IdentityUserToken<string>>(b => b.ToTable("UserTokens"));
        modelBuilder.Entity<IdentityRole>(b => b.ToTable("Roles"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims"));
        modelBuilder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRoles"));
    }
}
