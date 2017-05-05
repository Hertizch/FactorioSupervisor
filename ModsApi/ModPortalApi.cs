using ModsApi.Extensions;
using ModsApi.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace ModsApi
{
    /* example request string
     * https://mods.factorio.com/api/mods?page_size=max&namelist=aai-programmable-vehicles
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

        public ApiData ApiData { get; set; } = new ApiData();

        public async Task BuildApiData(string request)
        {
            Debug.WriteLine($"Method called: {nameof(BuildApiData)}");

            Exception exception = null;

            try
            {
                _json = await GetApiResponseJsonString(request);
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

            if (!string.IsNullOrEmpty(_json))
                ApiData = JsonConvert.DeserializeObject<ApiData>(_json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private async Task<string> GetApiResponseJsonString(string request)
        {
            Debug.WriteLine($"Method called: {nameof(GetApiResponseJsonString)}, param name: {nameof(request)}, value: {request}");

            Exception exception = null;
            string response = null;

            using (_webClient = new PatientWebClient { Proxy = null })
            {
                try
                {
                    response = await _webClient.DownloadStringTaskAsync($"{ApiResponseUrl}{request}");
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Debug.WriteLine($"Method execution failed: {nameof(GetApiResponseJsonString)}, exception message: {ex.Message}");
                }
                finally
                {
                    if (exception == null)
                        Debug.WriteLine($"Method execution succeeded: {nameof(GetApiResponseJsonString)}");
                }
            }

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
