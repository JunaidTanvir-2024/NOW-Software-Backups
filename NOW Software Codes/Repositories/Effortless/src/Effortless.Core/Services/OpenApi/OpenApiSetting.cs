namespace Effortless.Core.Services.OpenApi;

public sealed class OpenApiSetting
{
    public const string SectionName = nameof(OpenApiSetting);
    public required string Version { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string TermsOfService { get; set; }
    public required string ContactName { get; set; }
    public required string ContactUrl { get; set; }
    public required string LicenseName { get; set; }
    public required string LicenseUrl { get; set; }
    public required string JwtSecurityDefinitionName { get; set; }
    public required string JwtSecurityDefinitionDescription { get; set; }
    public required string JwtSecurityDefinitionBearerFormat { get; set; }
}
