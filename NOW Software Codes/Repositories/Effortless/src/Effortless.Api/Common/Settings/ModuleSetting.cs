namespace Effortless.Api.Common.Settings;


public sealed class ModuleSetting
{
    public bool IsEnable { get; set; }
    public string Name { get; set; } = null!;
    public string Prefix { get; set; } = null!;
}

