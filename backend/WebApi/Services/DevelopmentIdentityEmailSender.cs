using Microsoft.AspNetCore.Identity;

namespace WebApi.Services;

public sealed class DevelopmentIdentityEmailSender : IEmailSender<IdentityUser>
{
    private readonly ILogger<DevelopmentIdentityEmailSender> _logger;

    public DevelopmentIdentityEmailSender(ILogger<DevelopmentIdentityEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        _logger.LogInformation("Development email confirmation link for {Email}: {ConfirmationLink}", email, confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        _logger.LogInformation("Development password reset link for {Email}: {ResetLink}", email, resetLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        _logger.LogInformation("Development password reset code for {Email}: {ResetCode}", email, resetCode);
        return Task.CompletedTask;
    }
}
