using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Email template management.</summary>
    public class Templates
    {
        private readonly VeilMailHttpClient _http;

        internal Templates(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/templates", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/templates", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/templates/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PatchAsync($"/v1/templates/{id}", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> PreviewAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync("/v1/templates/preview", parameters, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/templates/{id}", ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
