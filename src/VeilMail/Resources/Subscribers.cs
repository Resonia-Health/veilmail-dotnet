using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Subscriber management within an audience.</summary>
    public class Subscribers
    {
        private readonly VeilMailHttpClient _http;
        private readonly string _basePath;

        internal Subscribers(VeilMailHttpClient http, string audienceId)
        {
            _http = http;
            _basePath = $"/v1/audiences/{audienceId}/subscribers";
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync(_basePath, parameters, ct);
        }

        public async Task<Dictionary<string, object?>> AddAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PostAsync(_basePath, parameters, ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string subscriberId, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"{_basePath}/{subscriberId}", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string subscriberId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            var response = await _http.PutAsync($"{_basePath}/{subscriberId}", parameters, ct);
            return Unwrap(response);
        }

        public async Task RemoveAsync(string subscriberId, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"{_basePath}/{subscriberId}", ct);
        }

        public async Task<Dictionary<string, object?>> ConfirmAsync(string subscriberId, CancellationToken ct = default)
        {
            var response = await _http.PostAsync($"{_basePath}/{subscriberId}/confirm", ct: ct);
            return Unwrap(response);
        }

        public async Task<Dictionary<string, object?>> ImportAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync($"{_basePath}/import", parameters, ct);
        }

        public async Task<string> ExportAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetRawAsync($"{_basePath}/export", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> ActivityAsync(string subscriberId, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"{_basePath}/{subscriberId}/activity", parameters, ct);
        }

        private static Dictionary<string, object?> Unwrap(Dictionary<string, object?> response)
        {
            if (response.TryGetValue("data", out var data) && data is Dictionary<string, object?> dict)
                return dict;
            return response;
        }
    }
}
