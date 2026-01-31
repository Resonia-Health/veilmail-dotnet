using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Contact property management.</summary>
    public class Properties
    {
        private readonly VeilMailHttpClient _http;

        internal Properties(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/properties", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/properties", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/properties/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PatchAsync($"/v1/properties/{id}", parameters, ct);
            return Unwrap(response);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/properties/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> GetValuesAsync(string audienceId, string subscriberId, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/audiences/{audienceId}/subscribers/{subscriberId}/properties", ct: ct);
        }

        public async Task<Dictionary<string, object?>> SetValuesAsync(string audienceId, string subscriberId, Dictionary<string, object?> values, CancellationToken ct = default)
        {
            return await _http.PutAsync($"/v1/audiences/{audienceId}/subscribers/{subscriberId}/properties", values, ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
