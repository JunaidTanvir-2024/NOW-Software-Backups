namespace Effortless.Api.Features.Auth.Otp.Response;

public class Response
{
    public long? Code { get; set; }
    public string? UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? ExpiryTime { get; set; }
    public int Type { get; set; }
}
