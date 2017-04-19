using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace FactorioSupervisor.Helpers
{
    public static class JsonHelpers
    {
        public static string GetModInfoJsonString(string filename, bool isCompressed = true)
        {
            string infoJson = null;

            if (!isCompressed)
            {
                if (!File.Exists(Path.Combine(filename, "info.json"))) return null;

                try
                {
                    infoJson = File.ReadAllText(Path.Combine(filename, "info.json"));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Faile to read file: {Path.Combine(filename, "info.json")}", true, ex);
                }
            }
            else
            {
                try
                {
                    using (var zipFile = ZipFile.Read(filename))
                    {
                        foreach (var zipEntry in zipFile.Entries.Where(zipEntry => zipEntry.FileName.EndsWith("/info.json")))
                        {
                            using (var ms = new MemoryStream())
                            {
                                zipEntry.Extract(ms);

                                ms.Position = 0;

                                using (var streamReader = new StreamReader(ms))
                                    infoJson = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Failed to read zip archive: {filename}", true, ex);
                }
            }

            return infoJson;
        }
    }
}
