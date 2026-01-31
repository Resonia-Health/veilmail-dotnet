using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VeilMail.Exceptions;

namespace VeilMail
{
    /// <summary>
    /// Internal HTTP client for communicating with the Veil Mail API.
    /// </summary>
    internal class VeilMailHttpClient
    {
        private const string DefaultBaseUrl = "https://api.veilmail.xyz";
        private const string Version = "0.1.0";

        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly string _baseUrl;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };

        internal VeilMailHttpClient(string apiKey, string? baseUrl, TimeSpan? timeout, System.Net.Http.HttpClient? httpClient)
        {
            _baseUrl = (baseUrl ?? DefaultBaseUrl).TrimEnd('/');
            _httpClient = httpClient ?? new System.Net.Http.HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"veilmail-dotnet/{Version}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (timeout.HasValue)
            {
                _httpClient.Timeout = timeout.Value;
            }
        }

        internal async Task<Dictionary<string, object?>> GetAsync(string path, Dictionary<string, object?>? query = null, CancellationToken ct = default)
        {
            return await RequestAsync(HttpMethod.Get, path, null, query, ct);
        }

        internal async Task<Dictionary<string, object?>> PostAsync(string path, Dictionary<string, object?>? body = null, CancellationToken ct = default)
        {
            return await RequestAsync(HttpMethod.Post, path, body, null, ct);
        }

        internal async Task<Dictionary<string, object?>> PatchAsync(string path, Dictionary<string, object?>? body = null, CancellationToken ct = default)
        {
            return await RequestAsync(new HttpMethod("PATCH"), path, body, null, ct);
        }

        internal async Task<Dictionary<string, object?>> PutAsync(string path, Dictionary<string, object?>? body = null, CancellationToken ct = default)
        {
            return await RequestAsync(HttpMethod.Put, path, body, null, ct);
        }

        internal async Task<Dictionary<string, object?>> DeleteAsync(string path, CancellationToken ct = default)
        {
            return await RequestAsync(HttpMethod.Delete, path, null, null, ct);
        }

        internal async Task<string> GetRawAsync(string path, Dictionary<string, object?>? query = null, CancellationToken ct = default)
        {
            var url = BuildUrl(path, query);
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                ThrowApiError((int)response.StatusCode, errorBody);
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<Dictionary<string, object?>> RequestAsync(
            HttpMethod method, string path, Dictionary<string, object?>? body,
            Dictionary<string, object?>? query, CancellationToken ct)
        {
            var url = BuildUrl(path, query);
            var request = new HttpRequestMessage(method, url);

            if (body != null && (method == HttpMethod.Post || method.Method == "PATCH" || method == HttpMethod.Put))
            {
                var filtered = FilterNulls(body);
                var json = JsonSerializer.Serialize(filtered, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request, ct);

            if ((int)response.StatusCode == 204)
            {
                return new Dictionary<string, object?>();
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ThrowApiError((int)response.StatusCode, responseBody);
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                return new Dictionary<string, object?>();
            }

            return DeserializeResponse(responseBody);
        }

        private string BuildUrl(string path, Dictionary<string, object?>? query)
        {
            var sb = new StringBuilder(_baseUrl).Append(path);

            if (query != null && query.Count > 0)
            {
                var filtered = query.Where(kv => kv.Value != null).ToList();
                if (filtered.Count > 0)
                {
                    sb.Append('?');
                    sb.Append(string.Join("&", filtered.Select(kv =>
                    {
                        var value = kv.Value is bool b ? (b ? "true" : "false") : kv.Value!.ToString();
                        return $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(value)}";
                    })));
                }
            }

            return sb.ToString();
        }

        private static Dictionary<string, object?> FilterNulls(Dictionary<string, object?> data)
        {
            var result = new Dictionary<string, object?>();
            foreach (var kv in data)
            {
                if (kv.Value == null) continue;
                if (kv.Value is Dictionary<string, object?> nested)
                {
                    result[kv.Key] = FilterNulls(nested);
                }
                else
                {
                    result[kv.Key] = kv.Value;
                }
            }
            return result;
        }

        private static Dictionary<string, object?> DeserializeResponse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return ElementToDict(doc.RootElement);
        }

        private static Dictionary<string, object?> ElementToDict(JsonElement element)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = ElementToValue(prop.Value);
            }
            return dict;
        }

        private static object? ElementToValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ElementToDict(element),
                JsonValueKind.Array => element.EnumerateArray().Select(ElementToValue).ToList(),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => null,
            };
        }

        private static void ThrowApiError(int statusCode, string responseBody)
        {
            var message = $"HTTP error {statusCode}";
            string? code = null;
            Dictionary<string, object>? details = null;
            List<string>? piiTypes = null;
            int? retryAfter = null;

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;
                var error = root.TryGetProperty("error", out var errorProp) ? errorProp : root;

                if (error.TryGetProperty("message", out var msgProp))
                    message = msgProp.GetString() ?? message;
                if (error.TryGetProperty("code", out var codeProp))
                    code = codeProp.GetString();
                if (error.TryGetProperty("piiTypes", out var piiProp))
                    piiTypes = piiProp.EnumerateArray().Select(e => e.GetString()!).ToList();
                if (error.TryGetProperty("retryAfter", out var retryProp))
                    retryAfter = retryProp.GetInt32();
            }
            catch
            {
                // Use defaults
            }

            throw statusCode switch
            {
                400 => new Exceptions.ValidationException(message, code, details),
                401 => new Exceptions.AuthenticationException(message, code, details),
                403 => new Exceptions.ForbiddenException(message, code, details),
                404 => new Exceptions.NotFoundException(message, code, details),
                422 when code == "pii_detected" || piiTypes != null =>
                    new Exceptions.PiiDetectedException(message, piiTypes, code, details),
                429 => new Exceptions.RateLimitException(message, retryAfter, code, details),
                >= 500 => new Exceptions.ServerException(message, code, statusCode, details),
                _ => new Exceptions.VeilMailException(message, code, statusCode, details),
            };
        }
    }
}
