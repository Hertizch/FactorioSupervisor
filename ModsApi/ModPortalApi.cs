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

            _json = null;
            _responseJson = null;
            _buildApiDataException = null;
            _getApiResponseJsonStringException = null;
        }

        private WebClient _webClient;
        private string _json;
        private string _responseJson;
        private Exception _buildApiDataException;
        private Exception _getApiResponseJsonStringException;
        private const string ApiResponseUrl = "https://mods.factorio.com/api/mods";

        public ApiData ApiData { get; set; }

        public async Task BuildApiData(string request)
        {
            Debug.WriteLine($"Method called: {nameof(BuildApiData)}");

            try
            {
                _json = await GetApiResponseJsonString(request);
            }
            catch (Exception ex)
            {
                _buildApiDataException = ex;
                Debug.WriteLine($"Method execution failed: {nameof(BuildApiData)}, exception message: {ex.Message}");
            }
            finally
            {
                if (_buildApiDataException == null)
                    Debug.WriteLine($"Method execution succeeded: {nameof(BuildApiData)}");
            }

            if (!string.IsNullOrEmpty(_json))
                ApiData = JsonConvert.DeserializeObject<ApiData>(_json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            else
                Debug.WriteLine($"Failed to get {nameof(GetApiResponseJsonString)} with request: {request} - JSON is null");
        }

        private async Task<string> GetApiResponseJsonString(string request)
        {
            Debug.WriteLine($"Method called: {nameof(GetApiResponseJsonString)}, param name: {nameof(request)}, value: {request}");

            using (_webClient = new WebClient { Proxy = null })
            {
                _webClient.DownloadProgressChanged += (sender, args) => Debug.WriteLine($"_webClient.DownloadProgressChanged {args.BytesReceived}");
                _webClient.DownloadStringCompleted += (sender, args) => Debug.WriteLine($"_webClient.DownloadStringCompleted");

                try
                {
                    _responseJson = await _webClient.DownloadStringTaskAsync($"{ApiResponseUrl}{request}");
                }
                catch (Exception ex)
                {
                    _getApiResponseJsonStringException = ex;
                    Debug.WriteLine($"Method execution failed: {nameof(GetApiResponseJsonString)}, exception message: {ex.Message}");
                }
                finally
                {
                    if (_getApiResponseJsonStringException == null)
                        Debug.WriteLine($"Method execution succeeded: {nameof(GetApiResponseJsonString)}");
                }
            }

            return _responseJson;
        }
    }
}
