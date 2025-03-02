using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StarKNET.Logger {
    public interface ILogger {
        public void Log(string message);
        public void LogError(string message, Exception ex);
    }
    public class Logger : ILogger {
        private readonly LogConfiguration Configuration;
        private readonly object LogLock = new();
        private readonly string absoluteFilePath;

        public Logger(LogConfiguration? config = null) {
            if (config != null)
                this.Configuration = config;
            else
                this.Configuration = new LogConfiguration();
            if (!Directory.Exists(Configuration.LogDirectory)) {
                try {
                    Directory.CreateDirectory(Configuration.LogDirectory);
                }
                catch (Exception ex) {
                    Console.WriteLine("An exception occurred while creating log directory: " + ex.Message);
                    throw;
                }
            }
            absoluteFilePath = Path.Combine(Configuration.LogDirectory, Configuration.LogFile + ".log");
            try {
                if (File.Exists(absoluteFilePath))
                    File.Delete(absoluteFilePath);
            }
            catch (Exception ex) {
                Console.WriteLine("An exception occurred while deleting old log: " + ex.Message);
                throw;
            }
        }

        public void Log(string message) {
            lock (LogLock) {
                if (Configuration.IncludeTimestamps)
                    message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                try {
                    File.AppendAllText(absoluteFilePath, message + Environment.NewLine);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error occurred while writing to log: {ex.Message}");
                }
                if (Configuration.WriteToConsole)
                    Console.WriteLine(message);
            }
        }

        public void LogError(string message, Exception? ex = null) {
            string errorMessage = ex != null
                ? $"ERROR: {message} - Exception: {ex.Message}\nStackTrace: {ex.StackTrace}"
                : $"ERROR: {message}";
            Log(errorMessage);
        }

    }
    public static class LoggerExtensions {
    }
}
