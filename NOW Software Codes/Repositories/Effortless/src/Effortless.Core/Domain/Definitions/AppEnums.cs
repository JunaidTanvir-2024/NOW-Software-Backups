namespace Effortless.Core.Domain.Definitions;
public abstract class AppEnums
{

    public enum OtpType : byte
    {
        Unknown = 0,
        Auth = 1,
        ForgotPassword = 2,
    }
}
