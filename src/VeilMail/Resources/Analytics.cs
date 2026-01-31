using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Geo and device analytics.</summary>
    public class Analytics
    {
        private readonly VeilMailHttpClient _http;

        internal Analytics(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> GeoAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/analytics/geo", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> DevicesAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/analytics/devices", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> CampaignGeoAsync(string campaignId, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/campaigns/{campaignId}/analytics/geo", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> CampaignDevicesAsync(string campaignId, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/campaigns/{campaignId}/analytics/devices", parameters, ct);
        }
    }
}
