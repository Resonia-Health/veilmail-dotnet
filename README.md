# Veil Mail .NET SDK

Official .NET SDK for the [Veil Mail](https://veilmail.xyz) API. Send emails with built-in PII protection.

> **Veil Mail is a drop-in alternative to [Resend](https://veilmail.xyz/vs/resend), [SendGrid](https://veilmail.xyz/vs/sendgrid), [Mailgun](https://veilmail.xyz/vs/mailgun), and [Postmark](https://veilmail.xyz/vs/postmark)** for .NET and ASP.NET Core. Full support for emails, domains, templates, audiences, campaigns, webhooks, and topics — with automatic PII scanning and CASL compliance built in.
>
> **Migration guides:** [from Resend](https://veilmail.xyz/docs/guides/migrate-resend) · [from SendGrid](https://veilmail.xyz/docs/guides/migrate-sendgrid) · [from Mailgun](https://veilmail.xyz/docs/guides/migrate-mailgun) · [from Postmark](https://veilmail.xyz/docs/guides/migrate-postmark)

## Requirements

- .NET Standard 2.1+ / .NET 6+

## Installation

```bash
dotnet add package VeilMail
```

Or via NuGet Package Manager:

```powershell
Install-Package VeilMail
```

## Quick Start

```csharp
using VeilMail;

var client = new VeilMailClient("veil_live_xxxxx");

var email = await client.Emails.SendAsync(new Dictionary<string, object?>
{
    ["from"] = "hello@yourdomain.com",
    ["to"] = "user@example.com",
    ["subject"] = "Hello from .NET!",
    ["html"] = "<h1>Welcome!</h1>",
});

Console.WriteLine(email["id"]);     // email_xxxxx
Console.WriteLine(email["status"]); // queued
```

## Configuration

```csharp
var client = new VeilMailClient(
    apiKey: "veil_live_xxxxx",
    baseUrl: "https://custom-api.example.com",
    timeout: TimeSpan.FromSeconds(10)
);

// Or with a custom HttpClient
var httpClient = new HttpClient();
var client = new VeilMailClient("veil_live_xxxxx", httpClient: httpClient);
```

## Emails

```csharp
// Send with named sender
var email = await client.Emails.SendAsync(new Dictionary<string, object?>
{
    ["from"] = "Alice <alice@yourdomain.com>",
    ["to"] = new List<string> { "bob@example.com" },
    ["subject"] = "Hello",
    ["html"] = "<p>Hello Bob!</p>",
    ["tags"] = new List<string> { "welcome" },
});

// Send with template
var email = await client.Emails.SendAsync(new Dictionary<string, object?>
{
    ["from"] = "hello@yourdomain.com",
    ["to"] = "user@example.com",
    ["templateId"] = "tmpl_xxx",
    ["templateData"] = new Dictionary<string, object> { ["name"] = "Alice" },
});

// Batch send (up to 100)
var result = await client.Emails.SendBatchAsync(new List<Dictionary<string, object?>>
{
    new() { ["from"] = "hi@yourdomain.com", ["to"] = new List<string> { "u1@ex.com" }, ["subject"] = "Hi", ["html"] = "<p>Hi!</p>" },
    new() { ["from"] = "hi@yourdomain.com", ["to"] = new List<string> { "u2@ex.com" }, ["subject"] = "Hi", ["html"] = "<p>Hi!</p>" },
});

// List, get, cancel, reschedule
var emails = await client.Emails.ListAsync(new Dictionary<string, object?> { ["status"] = "delivered", ["limit"] = 10 });
var email = await client.Emails.GetAsync("email_xxx");
var result = await client.Emails.CancelAsync("email_xxx");
var updated = await client.Emails.UpdateAsync("email_xxx", new Dictionary<string, object?> { ["scheduledFor"] = "2025-07-01T09:00:00Z" });
```

## Domains

```csharp
var domain = await client.Domains.CreateAsync(new Dictionary<string, object?> { ["domain"] = "mail.example.com" });
domain = await client.Domains.VerifyAsync((string)domain["id"]!);

// Update tracking
domain = await client.Domains.UpdateAsync((string)domain["id"]!, new Dictionary<string, object?>
{
    ["trackOpens"] = true,
    ["trackClicks"] = true,
});

// List and delete
var domains = await client.Domains.ListAsync();
await client.Domains.DeleteAsync((string)domain["id"]!);
```

## Templates

```csharp
var template = await client.Templates.CreateAsync(new Dictionary<string, object?>
{
    ["name"] = "Welcome",
    ["subject"] = "Welcome, {{name}}!",
    ["html"] = "<h1>Hello {{name}}</h1>",
    ["variables"] = new List<Dictionary<string, object>>
    {
        new() { ["name"] = "name", ["type"] = "string", ["required"] = true },
    },
});
```

## Audiences & Subscribers

```csharp
var audience = await client.Audiences.CreateAsync(new Dictionary<string, object?> { ["name"] = "Newsletter" });
var subs = client.Audiences.Subscribers((string)audience["id"]!);

var subscriber = await subs.AddAsync(new Dictionary<string, object?>
{
    ["email"] = "user@example.com",
    ["firstName"] = "Alice",
    ["lastName"] = "Smith",
});

var subscribers = await subs.ListAsync(new Dictionary<string, object?> { ["status"] = "active", ["limit"] = 50 });
var csv = await subs.ExportAsync(new Dictionary<string, object?> { ["status"] = "active" });
```

## Campaigns

```csharp
var campaign = await client.Campaigns.CreateAsync(new Dictionary<string, object?>
{
    ["name"] = "Summer Sale",
    ["subject"] = "50% Off!",
    ["from"] = "Store <deals@yourdomain.com>",
    ["audienceId"] = "aud_xxx",
    ["html"] = "<h1>Summer Sale!</h1>",
});

await client.Campaigns.ScheduleAsync((string)campaign["id"]!, new Dictionary<string, object?> { ["scheduledAt"] = "2025-06-15T10:00:00Z" });
await client.Campaigns.SendAsync((string)campaign["id"]!);
await client.Campaigns.PauseAsync((string)campaign["id"]!);
await client.Campaigns.ResumeAsync((string)campaign["id"]!);
await client.Campaigns.CancelAsync((string)campaign["id"]!);
```

## Webhooks

```csharp
var webhook = await client.Webhooks.CreateAsync(new Dictionary<string, object?>
{
    ["url"] = "https://yourdomain.com/webhooks/veilmail",
    ["events"] = new List<string> { "email.delivered", "email.bounced" },
});

var result = await client.Webhooks.TestAsync((string)webhook["id"]!);
webhook = await client.Webhooks.RotateSecretAsync((string)webhook["id"]!);
```

### Signature Verification

```csharp
using VeilMail;

// In an ASP.NET controller
[HttpPost("webhooks/veilmail")]
public IActionResult HandleWebhook()
{
    using var reader = new StreamReader(Request.Body);
    var body = reader.ReadToEnd();
    var signature = Request.Headers["X-Signature-Hash"].FirstOrDefault() ?? "";

    if (!Webhook.VerifySignature(body, signature, _webhookSecret))
    {
        return Unauthorized();
    }

    var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
    // Process event...

    return Ok();
}
```

## Topics

```csharp
var topic = await client.Topics.CreateAsync(new Dictionary<string, object?>
{
    ["name"] = "Product Updates",
    ["isDefault"] = true,
});

var prefs = await client.Topics.GetPreferencesAsync("aud_xxx", "sub_xxx");
await client.Topics.SetPreferencesAsync("aud_xxx", "sub_xxx", new Dictionary<string, object?>
{
    ["topics"] = new List<Dictionary<string, object>>
    {
        new() { ["topicId"] = "topic_xxx", ["subscribed"] = true },
        new() { ["topicId"] = "topic_yyy", ["subscribed"] = false },
    },
});
```

## Contact Properties

```csharp
var prop = await client.Properties.CreateAsync(new Dictionary<string, object?>
{
    ["key"] = "company",
    ["name"] = "Company Name",
    ["type"] = "text",
});

await client.Properties.SetValuesAsync("aud_xxx", "sub_xxx", new Dictionary<string, object?>
{
    ["company"] = "Acme Corp",
});
var values = await client.Properties.GetValuesAsync("aud_xxx", "sub_xxx");
```

## Error Handling

```csharp
using VeilMail.Exceptions;

try
{
    await client.Emails.SendAsync(new Dictionary<string, object?>
    {
        ["from"] = "hello@yourdomain.com",
        ["to"] = "user@example.com",
        ["subject"] = "Hello",
        ["html"] = "<p>Hi!</p>",
    });
}
catch (RateLimitException e)
{
    Console.WriteLine($"Rate limited. Retry after {e.RetryAfter}s");
}
catch (PiiDetectedException e)
{
    Console.WriteLine($"PII detected: {string.Join(", ", e.PiiTypes)}");
}
catch (ValidationException e)
{
    Console.WriteLine($"Validation error: {e.Message}");
}
catch (AuthenticationException)
{
    Console.WriteLine("Invalid API key");
}
catch (VeilMailException e)
{
    Console.WriteLine($"API error: {e.Message} (code: {e.ErrorCode})");
}
```

## Cancellation Support

All async methods accept an optional `CancellationToken`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var email = await client.Emails.GetAsync("email_xxx", cts.Token);
```

## License

MIT
