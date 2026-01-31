using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Webhook endpoint management.</summary>
    public class Webhooks
    {
        private readonly VeilMailHttpClient _http;

        internal Webhooks(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/webhooks", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/webhooks", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/webhooks/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PatchAsync($"/v1/webhooks/{id}", parameters, ct);
            return Unwrap(response);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/webhooks/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> TestAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/webhooks/{id}/test", ct: ct);
        }

        public async Task<Dictionary<string, object?>> RotateSecretAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/webhooks/{id}/rotate-secret", ct: ct);
            return Unwrap(response);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
