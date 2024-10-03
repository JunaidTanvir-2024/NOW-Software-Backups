using Effortless.Api.Common.Settings;

namespace Effortless.Api.Features.Auth;

public class AuthFeatureSetting
{
    public AvailableEndpoints Endpoints { get; set; } = new AvailableEndpoints();
    public ModuleSetting Module { get; set; } = new ModuleSetting();

    public sealed class AvailableEndpoints
    {
        public EndpointSetting Login { get; set; } = new EndpointSetting();
        public EndpointSetting Otp { get; set; } = new EndpointSetting();
        public EndpointSetting Signup { get; set; } = new EndpointSetting();
        public EndpointSetting ForgotPassword { get; set; } = new EndpointSetting();
        public EndpointSetting OtpVerify { get; set; } = new EndpointSetting();
        public EndpointSetting RefreshToken { get; set; } = new EndpointSetting();
    }
}
