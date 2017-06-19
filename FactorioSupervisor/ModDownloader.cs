using FactorioSupervisor.Models;
using FactorioSupervisor.ViewModels;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FactorioSupervisor.Helpers;

namespace FactorioSupervisor
{
    public class ModDownloader
    {
        public ModDownloader(Mod mod)
        {
            _mod = mod;
            _tempModFilename = Path.Combine(BaseVm.ConfigVm.ModsPath, _mod?.RemoteFilename + ".PART");
        }

        public ModDownloader(Dependency dependency)
        {
            _dependency = dependency;
            _tempDependencyFilename = Path.Combine(BaseVm.ConfigVm.ModsPath, _dependency?.RemoteFilename + ".PART");
        }

        private readonly Mod _mod;
        private readonly Dependency _dependency;
        private WebClient _webClient;
        private Exception _exception;
        private readonly string _tempModFilename;
        private readonly string _tempDependencyFilename;

        public bool DownloadSuccessful { get; set; }

        public async Task ExecuteDownload()
        {
            using (_webClient = new WebClient { Proxy = null })
            {
                _webClient.DownloadProgressChanged += (sender, args) =>
                {
                    if (_mod != null)
                        _mod.ProgressPercentage = args.ProgressPercentage;
                    else if (_dependency != null)
                        _dependency.ProgressPercentage = args.ProgressPercentage;
                };

                try
                {
                    if (_mod != null)
                        await _webClient.DownloadFileTaskAsync($"https://mods.factorio.com{_mod.DownloadUrl}?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}", _tempModFilename);
                    else if (_dependency != null)
                        await _webClient.DownloadFileTaskAsync($"https://mods.factorio.com{_dependency.DownloadUrl}?username={BaseVm.ConfigVm.ModPortalUsername}&token={BaseVm.ConfigVm.ModPortalAuthToken}", _tempDependencyFilename);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    // Set DownloadSuccessful
                    DownloadSuccessful = _exception == null;

                    // add a little delay...
                    await Task.Delay(500);

                    // If success, rename file
                    if (DownloadSuccessful)
                    {
                        if (_mod != null)
                            File.Move(_tempModFilename, Path.Combine(BaseVm.ConfigVm.ModsPath, _mod.RemoteFilename));
                        else if (_dependency != null)
                            File.Move(_tempDependencyFilename, Path.Combine(BaseVm.ConfigVm.ModsPath, _dependency.RemoteFilename));
                    }
                    else
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
                if (!File.Exists(_tempModFilename)) return;

                try
                {
                    File.Delete(_tempModFilename);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeletePartialFile)} failed. Unable to delete file: {_tempModFilename}", true, ex);
                }
            }
            else if (_dependency != null)
            {
                if (!File.Exists(_tempDependencyFilename)) return;

                try
                {
                    File.Delete(_tempDependencyFilename);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"[ERROR] Method {nameof(DeletePartialFile)} failed. Unable to delete file: {_tempDependencyFilename}", true, ex);
                }
            }
        }
    }
}
