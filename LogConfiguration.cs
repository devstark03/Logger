﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarKNET.Logging {
    public class LogConfiguration {
        public string LogDirectory { get; set; }
        public string LogFile { get; set; }
        public bool IncludeTimestamps { get; set; }
        public bool WriteToConsole { get; set; }

        public LogConfiguration() {
            LogDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            LogFile = "application";
            IncludeTimestamps = true;
            WriteToConsole = true;
        }
    }
}
