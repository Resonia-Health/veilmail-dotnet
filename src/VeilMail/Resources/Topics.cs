using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Subscription topic management.</summary>
    public class Topics
    {
        private readonly VeilMailHttpClient _http;

        internal Topics(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync("/v1/topics", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/topics", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/topics/{id}", ct: ct);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PatchAsync($"/v1/topics/{id}", parameters, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/topics/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> GetPreferencesAsync(string audienceId, string subscriberId, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/audiences/{audienceId}/subscribers/{subscriberId}/topics", ct: ct);
        }

        public async Task<Dictionary<string, object?>> SetPreferencesAsync(string audienceId, string subscriberId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PutAsync($"/v1/audiences/{audienceId}/subscribers/{subscriberId}/topics", parameters, ct);
        }
    }
}
