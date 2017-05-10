using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace FactorioSupervisor.Helpers
{
    public static class Logger
    {
        private static readonly string _logFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".log");
        private static FileStream _fileStream;

        public static void WriteLine(string value, bool writeToFile = false, Exception exception = null)
        {
            try
            {
                _fileStream = new FileStream(_logFilename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                using (var streamWriter = new StreamWriter(_fileStream, Encoding.UTF8))
                {
                    if (exception != null)
                    {
                        Debug.WriteLine($"[{DateTime.Now}]: {value} [Exception message: {exception.Message}]");
                        if (writeToFile)
                            streamWriter.WriteLine($"[{DateTime.Now}]: {value} [Exception message: {exception.Message}]");
                    }
                    else
                    {
                        Debug.WriteLine($"[{DateTime.Now}]: {value}");
                        if (writeToFile)
                            streamWriter.WriteLine($"[{DateTime.Now}]: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logger error: {ex}");
            }
            finally
            {
                _fileStream?.Dispose();
            }
        }

        private static void TrimFile()
        {
            var log = File.ReadAllLines(_logFilename);
        }
    }
}
