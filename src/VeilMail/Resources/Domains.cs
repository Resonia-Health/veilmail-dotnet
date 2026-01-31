using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Domain management for email sending.</summary>
    public class Domains
    {
        private readonly VeilMailHttpClient _http;

        internal Domains(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/domains", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/domains", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/domains/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PatchAsync($"/v1/domains/{id}", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> VerifyAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"/v1/domains/{id}/verify", ct: ct);
            return Unwrap(response);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/domains/{id}", ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
