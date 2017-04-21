using ModsApi.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace ModsApi
{
    /*
     * https://mods.factorio.com/api/mods?namelist=orechaos&namelist=power-armor
     */

    public class ModPortalApi
    {
        public ModPortalApi()
        {
            Debug.WriteLine($"Class created: {nameof(ModPortalApi)}");
        }

        private WebClient _webClient;
        private string _json;
        private const string ApiResponseUrl = "https://mods.factorio.com/api/mods";
        public ApiData ApiData { get; set; }

        public async Task BuildApiData(string request)
        {
            Debug.WriteLine($"Method called: {nameof(BuildApiData)}");

            Exception exception = null;

            // Get the api json response string
            try
            {
                _json = await GetApiResponseJson(request);
            }
            catch (Exception ex)
            {
                exception = ex;
                Debug.WriteLine($"Method execution failed: {nameof(BuildApiData)}, exception message: {ex.Message}");
            }
            finally
            {
                if (exception == null)
                    Debug.WriteLine($"Method execution succeeded: {nameof(BuildApiData)}");
            }

            // Deserialize the json
            ApiData = JsonConvert.DeserializeObject<ApiData>(_json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private async Task<string> GetApiResponseJson(string request)
        {
            Debug.WriteLine($"Method called: {nameof(GetApiResponseJson)}, param name: {nameof(request)}, value: {request}");

            Exception exception = null;

            // Initialize our response string
            string response = null;

            // Create the webclient
            using (_webClient = new WebClient { Proxy = null })
            {
                try
                {
                    response = await _webClient.DownloadStringTaskAsync($"{ApiResponseUrl}{request}");
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Debug.WriteLine($"Method execution failed: {nameof(GetApiResponseJson)}, exception message: {ex.Message}");
                }
                finally
                {
                    if (exception == null)
                        Debug.WriteLine($"Method execution succeeded: {nameof(GetApiResponseJson)}");
                }
            }

            // Return the response
            return response;
        }

        public async Task<bool> CanReachApi()
        {
            HttpWebResponse response = null;
            var result = false;

            if (WebRequest.Create(ApiResponseUrl) is HttpWebRequest request)
            {
                request.Method = WebRequestMethods.Http.Head;
                request.Timeout = 10000;

                try
                {
                    response = await request.GetResponseAsync() as HttpWebResponse;
                }
                catch
                {
                    // ignored
                }
            }

            if (response != null && response.StatusCode == HttpStatusCode.OK)
                result = true;

            return result;
        }
    }
}
