
using System;
using System.IO;
using System.Text;


namespace StarKNET.Logger {

    /// <summary>
    /// TextWriter implementation that redirects console output to the Logger
    /// </summary>
    public class ConsoleRedirector : TextWriter {
        private readonly ILogger _logger;
        private StringBuilder _lineBuffer = new StringBuilder();
        private readonly TextWriter _originalOut;

        public ConsoleRedirector(ILogger logger, TextWriter originalOut) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _originalOut = originalOut ?? throw new ArgumentNullException(nameof(originalOut));
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value) {
            _lineBuffer.Append(value);
            _originalOut.Write(value);  // Still write to the original console

            if (value == '\n') {
                // When we hit a newline, log the buffered content and clear it
                string line = _lineBuffer.ToString().TrimEnd('\r', '\n');
                if (!string.IsNullOrEmpty(line)) {
                    _logger.Log($"[Console] {line}");
                }
                _lineBuffer.Clear();
            }
        }

        public override void Write(string value) {
            if (string.IsNullOrEmpty(value)) return;

            // Handle multiple lines in a single write
            string[] lines = value.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++) {
                _lineBuffer.Append(lines[i]);

                // Still write to the original console
                _originalOut.Write(lines[i]);

                // If this isn't the last line, or the original string ended with a line break
                if (i < lines.Length - 1 || value.EndsWith("\n") || value.EndsWith("\r")) {
                    string line = _lineBuffer.ToString();
                    if (!string.IsNullOrEmpty(line)) {
                        _logger.Log($"[Console] {line}");
                    }
                    _lineBuffer.Clear();

                    if (i < lines.Length - 1) {
                        _originalOut.WriteLine();
                    }
                }
            }
        }

        public override void Flush() {
            // If there's anything buffered when Flush is called, log it
            if (_lineBuffer.Length > 0) {
                string line = _lineBuffer.ToString();
                _logger.Log($"[Console] {line}");
                _lineBuffer.Clear();
            }
            _originalOut.Flush();
        }

        public override void Close() {
            Flush();
            base.Close();
        }
    }
}