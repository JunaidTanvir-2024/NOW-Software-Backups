using Effortless.Api.Common.Settings;

namespace Effortless.Api.Features.User;

public class UserFeatureSetting
{
    public AvailableEndpoints Endpoints { get; set; } = new AvailableEndpoints();
    public ModuleSetting Module { get; set; } = new ModuleSetting();

    public sealed class AvailableEndpoints
    {
        public EndpointSetting Users { get; set; } = new EndpointSetting();
        public EndpointSetting UserByPhoneNumber { get; set; } = new EndpointSetting();
        public EndpointSetting Add { get; set; } = new EndpointSetting();
        public EndpointSetting Update { get; set; } = new EndpointSetting();
        public EndpointSetting Remove { get; set; } = new EndpointSetting();
    }
}
