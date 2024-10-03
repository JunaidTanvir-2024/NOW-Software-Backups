namespace Effortless.Core.Services.Jwt;
public sealed class JwtSetting
{
    public const string SectionName = nameof(JwtSetting);
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpirationTimeInMinutes { get; set; }
    public bool IsExactTimeExpiry { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public AsymmetricFilesInfo AsymmetricFiles { get; set; } = new AsymmetricFilesInfo();
    public sealed class AsymmetricFilesInfo
    {
        public const string SectionName = nameof(AsymmetricFiles);
        public static AsymmetricFilesInfo Instance { get; } = new AsymmetricFilesInfo();
        public string SecretKeyFile { get; set; } = null!;
        public string PublicKeyFile { get; set; } = null!;
    }
}
