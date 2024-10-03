using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Effortless.Core.Services.Otp;

public interface IOtpService
{
    long GenerateOtp();
}

internal sealed class OtpService : IOtpService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly OtpSetting _otpSettings;

    public OtpService(
        IHostEnvironment hostEnvironment,
        IOptions<OtpSetting> options)
    {
        _hostEnvironment = hostEnvironment;
        _otpSettings = options.Value;
    }
    public long GenerateOtp()
    {
        if (_hostEnvironment.IsDevelopment() || _hostEnvironment.IsStaging())
        {
            return _otpSettings.DefaultCode;
        }
        Random _rdm = new Random();
        return _rdm.NextInt64(_otpSettings.MinValue, _otpSettings.MaxValue);
    }
}
