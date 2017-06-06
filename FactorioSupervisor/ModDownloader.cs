using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FactorioSupervisor.Helpers;

namespace FactorioSupervisor
{
    public class ModDownloader
    {
        public ModDownloader([Optional] Mod mod, [Optional] Mod currentMod, [Optional] Dependency dependency)
        {
            _mod = mod;
            _currentMod = mod;
            _dependency = dependency;
        }

        private readonly Mod _mod;
        private readonly Mod _currentMod;
        private readonly Dependency _dependency;
        private WebClient _webClient;
        private Exception _exception;

        public bool DownloadSuccessful { get; set; }

        public async Task ExecuteDownload()
        {
            using (_webClient = new WebClient { Proxy = null })
            {
                _webClient.DownloadProgressChanged += (sender, args) =>
                {
                    if (_mod != null)
                    {
                        _mod.ProgressPercentage = args.ProgressPercentage;
                        _currentMod.ProgressPercentage = args.ProgressPercentage;
                    }
                    else if (_dependency != null)
                        _dependency.ProgressPercentage = args.ProgressPercentage;
                };

                try
                {
                    if (_mod != null)
                        await _webClient.DownloadFileTaskAsync($"https://mods.factorio.com{_mod.DownloadUrl}?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}", Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename));
                    else if (_dependency != null)
                        await _webClient.DownloadFileTaskAsync($"https://mods.factorio.com{_dependency.DownloadUrl}?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}", Path.Combine(BaseVm.ConfigVm.ModsPath, _dependency.RemoteFilename));
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    // Set DownloadSuccessful
                    DownloadSuccessful = _exception == null;

                    // If unsuccessful - delete partial file
                    if (!DownloadSuccessful)
                        DeletePartialFile();
                }
            }
        }

        public void DeleteOldFile()
        {
            if (_mod != null)
            {
                if (!File.Exists(_mod.FullName)) return;

                try
                {
                    File.Delete(_mod.FullName);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeleteOldFile)} failed. Unable to delete file: {_dependency.FullName}", true, ex);
                }
            }
            else if (_dependency != null)
            {
                if (!File.Exists(_dependency.FullName)) return;

                try
                {
                    File.Delete(_dependency.FullName);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeleteOldFile)} failed. Unable to delete file: {_dependency.FullName}", true, ex);
                }
            }
        }

        private void DeletePartialFile()
        {
            if (_mod != null)
            {
                if (!File.Exists(Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename))) return;

                try
                {
                    File.Delete(Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeletePartialFile)} failed. Unable to delete file: {Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename)}", true, ex);
                }
            }
            else if (_dependency != null)
            {
                if (!File.Exists(Path.Combine(BaseVm.ConfigVm.ModsPath, _dependency.RemoteFilename))) return;

                try
                {
                    File.Delete(Path.Combine(BaseVm.ConfigVm.ModsPath, _dependency.RemoteFilename));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeletePartialFile)} failed. Unable to delete file: {Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename)}", true, ex);
                }
            }
        }
    }
}
