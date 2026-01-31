using VeilMail;

namespace AspNetAuth.Services;

public class VeilMailService
{
    private readonly VeilMailClient _client;
    private readonly string _from;
    private readonly string _appUrl;

    public VeilMailService(IConfiguration config)
    {
        _client = new VeilMailClient(config["VeilMail:ApiKey"]!);
        _from = config["VeilMail:FromEmail"] ?? "noreply@veilmail.xyz";
        _appUrl = config["App:Url"] ?? "http://localhost:5000";
    }

    public async Task SendVerificationEmail(string email, string name, string token)
    {
        var url = $"{_appUrl}/auth/verify-email?token={token}";
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = "Verify your email address",
            Html = $"<p>Hi {name},</p><p>Click <a href=\"{url}\">here</a> to verify your email.</p>",
            Tags = ["auth", "verification"],
            Type = "transactional"
        });
    }

    public async Task SendPasswordResetEmail(string email, string token)
    {
        var url = $"{_appUrl}/auth/reset-password?token={token}";
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = "Reset your password",
            Html = $"<p>Click <a href=\"{url}\">here</a> to reset your password.</p>",
            Tags = ["auth", "password-reset"],
            Type = "transactional"
        });
    }

    public async Task SendTwoFactorCode(string email, string code)
    {
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = $"{code} is your verification code",
            Html = $"<p>Your code: <strong>{code}</strong></p><p>Expires in 5 minutes.</p>",
            Tags = ["auth", "2fa"],
            Type = "transactional"
        });
    }

    public async Task SendWelcomeEmail(string email, string name)
    {
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = "Welcome!",
            Html = $"<p>Welcome, {name}! Your account is active.</p>",
            Tags = ["auth", "welcome"],
            Type = "transactional"
        });
    }

    public async Task SendPasswordChangedEmail(string email)
    {
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = "Your password was changed",
            Html = "<p>Your password was changed. If you didn't do this, reset it immediately.</p>",
            Tags = ["auth", "security"],
            Type = "transactional"
        });
    }

    public async Task SendTwoFactorToggledEmail(string email, bool enabled)
    {
        var status = enabled ? "enabled" : "disabled";
        await _client.Emails.SendAsync(new SendEmailRequest
        {
            From = _from,
            To = email,
            Subject = $"Two-factor authentication {status}",
            Html = $"<p>2FA has been {status} on your account.</p>",
            Tags = ["auth", "2fa", "security"],
            Type = "transactional"
        });
    }
}
