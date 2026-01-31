using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeilMail.Resources
{
    /// <summary>Automation sequence management.</summary>
    public class Sequences
    {
        private readonly VeilMailHttpClient _http;

        internal Sequences(VeilMailHttpClient http) => _http = http;

        public async Task<Dictionary<string, object?>> CreateAsync(Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync("/v1/sequences", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> ListAsync(Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync("/v1/sequences", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> GetAsync(string id, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/sequences/{id}", ct: ct);
        }

        public async Task<Dictionary<string, object?>> UpdateAsync(string id, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PutAsync($"/v1/sequences/{id}", parameters, ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/sequences/{id}", ct);
        }

        public async Task<Dictionary<string, object?>> ActivateAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/sequences/{id}/activate", ct: ct);
        }

        public async Task<Dictionary<string, object?>> PauseAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/sequences/{id}/pause", ct: ct);
        }

        public async Task<Dictionary<string, object?>> ArchiveAsync(string id, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/sequences/{id}/archive", ct: ct);
        }

        public async Task<Dictionary<string, object?>> AddStepAsync(string sequenceId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/sequences/{sequenceId}/steps", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> UpdateStepAsync(string sequenceId, string stepId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PutAsync($"/v1/sequences/{sequenceId}/steps/{stepId}", parameters, ct);
        }

        public async Task DeleteStepAsync(string sequenceId, string stepId, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/sequences/{sequenceId}/steps/{stepId}", ct);
        }

        public async Task ReorderStepsAsync(string sequenceId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            await _http.PostAsync($"/v1/sequences/{sequenceId}/steps/reorder", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> EnrollAsync(string sequenceId, Dictionary<string, object?> parameters, CancellationToken ct = default)
        {
            return await _http.PostAsync($"/v1/sequences/{sequenceId}/enroll", parameters, ct);
        }

        public async Task<Dictionary<string, object?>> ListEnrollmentsAsync(string sequenceId, Dictionary<string, object?>? parameters = null, CancellationToken ct = default)
        {
            return await _http.GetAsync($"/v1/sequences/{sequenceId}/enrollments", parameters, ct);
        }

        public async Task RemoveEnrollmentAsync(string sequenceId, string enrollmentId, CancellationToken ct = default)
        {
            await _http.DeleteAsync($"/v1/sequences/{sequenceId}/enrollments/{enrollmentId}", ct);
        }
    }
}
