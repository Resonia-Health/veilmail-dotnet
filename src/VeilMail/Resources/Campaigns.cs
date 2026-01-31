using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Campaign management.</summary>
    public class Campaigns
    {
        private readonly VeilMailHttpClient _http;

        internal Campaigns(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/campaigns", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/campaigns", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/campaigns/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PatchAsync($"/v1/campaigns/{id}", parameters, ct);
            return Unwrap(response);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/campaigns/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> ScheduleAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/campaigns/{id}/schedule", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> SendAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/campaigns/{id}/send", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> PauseAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/campaigns/{id}/pause", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ResumeAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/campaigns/{id}/resume", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> CancelAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/campaigns/{id}/cancel", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> SendTestAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/campaigns/{id}/test", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> CloneAsync(string id, Dictionary<string, object?>? options = null, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/campaigns/{id}/clone", options ?? new Dictionary<string, object?>(), ct);
        }

        public async Task<Dictionary<string, object?>> LinksAsync(string id, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/campaigns/{id}/links", parameters, ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
