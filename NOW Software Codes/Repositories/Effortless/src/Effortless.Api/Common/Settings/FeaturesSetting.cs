using Effortless.Api.Features.Auth;
using Effortless.Api.Features.User;

namespace Effortless.Api.Common.Settings;

public partial class FeaturesSetting
{
    public const string SectionName = nameof(FeaturesSetting);
    public static FeaturesSetting Instance { get; } = new FeaturesSetting();

    // Modules or Features
    public AuthFeatureSetting Auth { get; set; } = new AuthFeatureSetting();
    public UserFeatureSetting User { get; set; } = new UserFeatureSetting();
}
