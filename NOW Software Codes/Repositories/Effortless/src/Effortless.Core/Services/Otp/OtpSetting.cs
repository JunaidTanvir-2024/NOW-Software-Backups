namespace Effortless.Core.Services.Otp;
public sealed class OtpSetting
{
    public const string SectionName = nameof(OtpSetting);
    public required long MinValue { get; set; }
    public required long MaxValue { get; set; }
    public required int BlockTimeInMinutes { get; set; }
    public required int ExpiryTimeInMinutes { get; set; }
    public required int CreationLimit { get; set; }
    public required long DefaultCode { get; set; }
}
