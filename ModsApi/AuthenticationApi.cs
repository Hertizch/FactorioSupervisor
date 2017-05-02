using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModsApi
{
    public class AuthenticationApi
    {
        public AuthenticationApi()
        {
            Debug.WriteLine($"Class created: {nameof(AuthenticationApi)}");
        }

        private static WebClient _webClient;
        private const string AuthenticationBaseUrl = "https://auth.factorio.com/api-login";

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<string> GetAuthenticationToken(string userName, string password, bool requireGameOwnership)
        {
            Debug.WriteLine($"Method called: {nameof(GetAuthenticationToken)}, param name: {nameof(userName)}, value: {userName}, param name: {nameof(password)}, value: [private], param name: {nameof(requireGameOwnership)}, value: {requireGameOwnership}");

            // Initialize the response as a byte array
            byte[] response = { };

            // Set our upload request parameters
            var nameValues = new NameValueCollection
            {
                ["require_game_ownership"] = requireGameOwnership ? "1" : "0",
                ["username"] = userName,
                ["password"] = password
            };

            // Create the webclient
            using (_webClient = new WebClient { Proxy = null })
            {
                try
                {
                    response = await _webClient.UploadValuesTaskAsync(AuthenticationBaseUrl, nameValues);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Method execution failed: {nameof(GetAuthenticationToken)}, exception message: {ex.Message}");

                    Success = false;
                    ErrorMessage = ex.Message;
                }
            }

            // If the response string is null, return null
            if (response == null) return null;

            // Convert byte array to string
            var responseString = Encoding.Default.GetString(response);

            // Deserialize json string to dynamic
            dynamic json = JsonConvert.DeserializeObject(responseString);

            // Get first object in json as string, aka our token
            var token = (string)json[0];

            // Set success as true
            Success = true;

            // Return the token
            return token;
        }
    }
}
