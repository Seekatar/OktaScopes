using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using app.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace app.Services
{
    public class OktaTokenService : ITokenService
    {
        private class OktaToken
        {
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; set; }

            public DateTime ExpiresAt { get; set; }

            public string Scope { get; set; }

            [JsonProperty(PropertyName = "token_type")]
            public string TokenType { get; set; }

            public bool IsValidAndNotExpiring
            {
                get
                {
                    return !String.IsNullOrEmpty(AccessToken) &&
                    ExpiresAt > DateTime.UtcNow.AddSeconds(30);
                }
            }
        }
        private OktaToken _token = new OktaToken();
        private readonly OktaSettings _oktaSettings;

        public OktaTokenService(IOptions<OktaSettings> oktaSettings)
        {
            _oktaSettings = oktaSettings.Value;
        }

        public async Task<string> GetToken()
        {
            if (!_token.IsValidAndNotExpiring)
            {
                _token = await GetNewAccessToken();
            }
            return _token.AccessToken;
        }
        private async Task<OktaToken> GetNewAccessToken()
        {
            // IMPORTANT, the example doesn’t tell you to add access_token to
            // the scopes of the default authorization
            // Goto to site and API->Authorization Servers->Scopes->Add Scope.

            _token = new OktaToken();
            var client = new HttpClient();
            var client_id = _oktaSettings.ClientId;
            var client_secret = _oktaSettings.ClientSecret;
            var clientCreds = System.Text.Encoding.UTF8.GetBytes($"{client_id}:{client_secret}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(clientCreds));

            var postMessage = new Dictionary<string, string>();
            postMessage.Add("grant_type", "client_credentials");
            postMessage.Add("scope", "access_token");
            var request = new HttpRequestMessage(HttpMethod.Post, _oktaSettings.TokenUrl)
            {
                Content = new FormUrlEncodedContent(postMessage)
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _token = JsonConvert.DeserializeObject<OktaToken>(json);
                _token.ExpiresAt = DateTime.UtcNow.AddSeconds(_token.ExpiresIn);
            }
            else
            {
                var msg = $"'{response.StatusCode}' returned from getting access token from Okta {Environment.NewLine}{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}";
                Console.WriteLine(msg);
                throw new ApplicationException(msg);
            }
            return _token;
        }
    }
}