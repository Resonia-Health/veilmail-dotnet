using System;
using System.Net.Http;
using VeilMail.Resources;

namespace VeilMail
{
    /// <summary>
    /// Veil Mail API client.
    /// </summary>
    /// <example>
    /// <code>
    /// var client = new VeilMailClient("veil_live_xxxxx");
    ///
    /// var email = await client.Emails.SendAsync(new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["from"] = "hello@yourdomain.com",
    ///     ["to"] = "user@example.com",
    ///     ["subject"] = "Hello from .NET!",
    ///     ["html"] = "&lt;h1&gt;Welcome!&lt;/h1&gt;",
    /// });
    /// </code>
    /// </example>
    public class VeilMailClient
    {
        public Emails Emails { get; }
        public Domains Domains { get; }
        public Templates Templates { get; }
        public Audiences Audiences { get; }
        public Campaigns Campaigns { get; }
        public Webhooks Webhooks { get; }
        public Topics Topics { get; }
        public Properties Properties { get; }
        public Sequences Sequences { get; }
        public Feeds Feeds { get; }
        public Forms Forms { get; }
        public Analytics Analytics { get; }

        /// <summary>
        /// Create a new Veil Mail client with default settings.
        /// </summary>
        /// <param name="apiKey">Your API key (must start with veil_live_ or veil_test_)</param>
        public VeilMailClient(string apiKey)
            : this(apiKey, null, null, null) { }

        /// <summary>
        /// Create a new Veil Mail client with custom settings.
        /// </summary>
        /// <param name="apiKey">Your API key (must start with veil_live_ or veil_test_)</param>
        /// <param name="baseUrl">Custom API base URL (null for default)</param>
        /// <param name="timeout">Request timeout (null for default)</param>
        /// <param name="httpClient">Custom HttpClient instance (null to create one)</param>
        public VeilMailClient(string apiKey, string? baseUrl = null, TimeSpan? timeout = null, HttpClient? httpClient = null)
        {
            if (string.IsNullOrEmpty(apiKey) ||
                (!apiKey.StartsWith("veil_live_") && !apiKey.StartsWith("veil_test_")))
            {
                throw new ArgumentException(
                    "Invalid API key format. Key must start with \"veil_live_\" or \"veil_test_\".",
                    nameof(apiKey)
                );
            }

            var http = new VeilMailHttpClient(apiKey, baseUrl, timeout, httpClient);

            Emails = new Emails(http);
            Domains = new Domains(http);
            Templates = new Templates(http);
            Audiences = new Audiences(http);
            Campaigns = new Campaigns(http);
            Webhooks = new Webhooks(http);
            Topics = new Topics(http);
            Properties = new Properties(http);
            Sequences = new Sequences(http);
            Feeds = new Feeds(http);
            Forms = new Forms(http);
            Analytics = new Analytics(http);
        }
    }
}
