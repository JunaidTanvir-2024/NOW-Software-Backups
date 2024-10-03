namespace Effortless.Core.Domain.Entities;

public class OtpEntity
{
    public long Id { get; set; }

    public long? Code { get; set; }

    public int Type { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public int UsageCount { get; set; }

    public bool IsAlreadyUsed { get; set; }

    public bool IsRetryLimitExceeded { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime? BlockTime { get; set; }

    public string? UserId { get; set; }

    public virtual UserEntity? User { get; set; }
}
