using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FactorioSupervisor
{
    public class ModDownloader
    {
        private WebClient _webClient;
        private Exception _exception;

        public bool DownloadSuccessful { get; set; }

        public async Task ExecuteDownload(Mod mod)
        {
            using (_webClient = new WebClient { Proxy = null })
            {
                _webClient.DownloadProgressChanged += (sender, args) => { mod.ProgressPercentage = args.ProgressPercentage; };

                // Execute download
                try
                {
                    await _webClient.DownloadFileTaskAsync($"https://mods.factorio.com{mod.DownloadUrl}?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}", Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename));
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    DownloadSuccessful = _exception == null;

                    if (DownloadSuccessful)
                        DownloadSuccessful = true;
                    else
                        if (File.Exists(Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename)))
                            File.Delete(Path.Combine(BaseVm.ConfigVm.ModsPath, mod.RemoteFilename));
                }
            }
        }
    }
}
