using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Brenner.SilentCmd
{
    internal class Configuration
    {
        public bool LogAppend { get; private set; }
        public string LogFilePath { get; private set; }
        public string BatchFilePath { get; set; }
        public string BatchFileArguments { get; private set; }
        public TimeSpan Delay { get; private set; }

        public void ParseArguments(IEnumerable<string> args)
        {
            var argumentsBuilder = new StringBuilder();
            var batchFilePathWasRead = false;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/LOG:", true, CultureInfo.InvariantCulture))
                {
                    LogAppend = false;
                    LogFilePath = arg.Substring(5).Trim('"');
                    continue;
                }

                if (arg.StartsWith("/LOG+:", true, CultureInfo.InvariantCulture))
                {
                    LogAppend = true;
                    LogFilePath = arg.Substring(6).Trim('"');
                    continue;
                }

                if (arg.StartsWith("/DELAY:", true, CultureInfo.InvariantCulture))
                {
                    string rawValue = arg.Substring(7).Trim('"');
                    Delay = TimeSpan.FromSeconds(Convert.ToDouble(rawValue));
                    continue;
                }

                if (!batchFilePathWasRead)
                {
                    BatchFilePath = arg;
                    batchFilePathWasRead = true;
                    continue;
                }

                if (arg.Contains(" "))
                {
                    argumentsBuilder.AppendFormat("\"{0}\" ", arg);
                    continue;
                }

                argumentsBuilder.AppendFormat("{0} ", arg);
            }

            if (argumentsBuilder.Length > 0)
            {
                BatchFileArguments = argumentsBuilder.ToString();
            }
        }

    }
}
