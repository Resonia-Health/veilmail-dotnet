using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Audience management.</summary>
    public class Audiences
    {
        private readonly VeilMailHttpClient _http;

        internal Audiences(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync("/v1/audiences", parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/audiences", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/v1/audiences/{id}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PutAsync($"/v1/audiences/{id}", parameters, ct);
            return Unwrap(response);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/audiences/{id}", ct);
        }

        /// <summary>Get a Subscribers helper scoped to the given audience.</summary>
        public Subscribers Subscribers(string audienceId) => new Subscribers(_http, audienceId);

        /// <summary>Recalculate engagement scores for all subscribers.</summary>
        public async Task<Dictionary<string, object?>> RecalculateEngagementAsync(string audienceId, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/audiences/{audienceId}/recalculate-engagement", new Dictionary<string, object?>(), ct);
        }

        /// <summary>Get engagement statistics for an audience.</summary>
        public async Task<Dictionary<string, object?>> GetEngagementStatsAsync(string audienceId, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/audiences/{audienceId}/engagement-stats", ct: ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
