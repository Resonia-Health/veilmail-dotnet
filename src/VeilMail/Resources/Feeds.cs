using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>RSS feed management.</summary>
    public class Feeds
    {
        private readonly VeilMailHttpClient _http;

        internal Feeds(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync("/v1/feeds", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> ListAsync(CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/feeds", ct: ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/feeds/{id}", ct: ct);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PutAsync($"/v1/feeds/{id}", parameters, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/feeds/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> PollAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/feeds/{id}/poll", ct: ct);
        }

        public async Task<Dictionary<string, object?>> PauseAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/feeds/{id}/pause", ct: ct);
        }

        public async Task<Dictionary<string, object?>> ResumeAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/feeds/{id}/resume", ct: ct);
        }

        public async Task<Dictionary<string, object?>> ListItemsAsync(string feedId, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/feeds/{feedId}/items", parameters, ct);
        }
    }
}
