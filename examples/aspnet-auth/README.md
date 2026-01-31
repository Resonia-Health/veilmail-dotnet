# ASP.NET Auth Example with VeilMail

Authentication email integration using the VeilMail .NET SDK in an ASP.NET application.

## Key Files

- `Services/VeilMailService.cs` - Mail service with auth email methods
- `appsettings.json` - Configuration for VeilMail API key and app settings

## Setup

1. Add the VeilMail NuGet package to your project
2. Copy the service and config files into your ASP.NET project
3. Register the service in `Program.cs`:
   ```csharp
   builder.Services.AddSingleton<VeilMailService>();
   ```
4. Update `appsettings.json` with your API key or set environment variables:
   ```bash
   export VeilMail__ApiKey=veil_live_your_key
   export VeilMail__FromEmail=noreply@yourdomain.com
   export App__Url=https://yourdomain.com
   ```
5. Inject `VeilMailService` into your auth controllers

## Emails Covered

- Email verification
- Password reset
- Two-factor authentication codes
- Welcome email
- Password changed notification
- 2FA toggled notification
