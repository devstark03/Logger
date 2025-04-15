using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StarKNET.Logger {
    public interface ILogger {
        void Log(string message);
        void LogError(string message, Exception ex);
    }
    public class Logger : ILogger, IDisposable {
        private readonly LogConfiguration Configuration;
        private readonly object LogLock = new object();
        private readonly string absoluteFilePath;
        private TextWriter originalConsoleOut;
        private ConsoleRedirector redirector;
        private bool disposedValue;

        public Logger(LogConfiguration config = null) {
            if (config != null) {
                Configuration = config;
            }
            else {
                Configuration = new LogConfiguration();
            }

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
                if (File.Exists(absoluteFilePath)) {
                    File.Delete(absoluteFilePath);
                }
            }
            catch (Exception ex2) {
                Console.WriteLine("An exception occurred while deleting old log: " + ex2.Message);
                throw;
            }
        }

        /// <summary>
        /// Starts capturing console output to the log
        /// </summary>
        public void CaptureConsoleOutput() {
            // Save original console output
            originalConsoleOut = Console.Out;

            // Create and set redirector
            redirector = new ConsoleRedirector(this, originalConsoleOut);
            Console.SetOut(redirector);
        }

        /// <summary>
        /// Stops capturing console output
        /// </summary>
        public void StopCaptureConsoleOutput() {
            if (redirector != null) {
                redirector.Flush();
                Console.SetOut(originalConsoleOut);
                redirector = null;
            }
        }

        public void Log(string message) {
            lock (LogLock) {
                if (Configuration.IncludeTimestamps) {
                    message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                }

                try {
                    File.AppendAllText(absoluteFilePath, message + Environment.NewLine);
                }
                catch (Exception ex) {
                    Console.WriteLine("Error occurred while writing to log: " + ex.Message);
                }

                if (Configuration.WriteToConsole && originalConsoleOut != null && redirector != null) {
                    // Write directly to the original console to avoid recursion
                    originalConsoleOut.WriteLine(message);
                }
                else if (Configuration.WriteToConsole) {
                    Console.WriteLine(message);
                }
            }
        }

        public void LogError(string message, Exception ex = null) {
            string message2 = ((ex != null) ?
                $"ERROR: {message} - Exception: {ex.Message}\nStackTrace: {ex.StackTrace}" :
                ("ERROR: " + message));
            Log(message2);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    StopCaptureConsoleOutput();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
