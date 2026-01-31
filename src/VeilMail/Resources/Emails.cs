using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Email sending and management.</summary>
    public class Emails
    {
        private readonly VeilMailHttpClient _http;

        internal Emails(VeilMailHttpClient http) => _http = http;

        /// <summary>Send a single email.</summary>
        public async Task<Dictionary<string, object?>> SendAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            // Ensure 'to' is always a list
            if (parameters.TryGetValue("to", out var to) && to is string toStr)
            {
                parameters["to"] = new List<string> { toStr };
            }
            if (parameters.TryGetValue("cc", out var cc) && cc is string ccStr)
            {
                parameters["cc"] = new List<string> { ccStr };
            }
            if (parameters.TryGetValue("bcc", out var bcc) && bcc is string bccStr)
            {
                parameters["bcc"] = new List<string> { bccStr };
            }
            return await _http.PostAsync("/v1/emails", parameters, ct);
        }

        /// <summary>Send a batch of up to 100 emails.</summary>
        public async Task<Dictionary<string, object?>> SendBatchAsync(List<Dictionary<string, object?>> emails, CancellationToken ct = default)
        {
            var body = new Dictionary<string, object?> { ["emails"] = emails };
            return await _http.PostAsync("/v1/emails/batch", body, ct);
        }

        /// <summary>List emails with optional filters.</summary>
        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/emails", parameters, ct);
        }

        /// <summary>Get a single email by ID.</summary>
        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/emails/{id}", ct: ct);
        }

        /// <summary>Cancel a scheduled email.</summary>
        public async Task<Dictionary<string, object?>> CancelAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/emails/{id}/cancel", ct: ct);
        }

        /// <summary>Reschedule a scheduled email.</summary>
        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PatchAsync($"/v1/emails/{id}", parameters, ct);
        }

        /// <summary>Get tracked link analytics for a specific email.</summary>
        public async Task<Dictionary<string, object?>> LinksAsync(string id, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/emails/{id}/links", parameters, ct);
        }
    }
}
