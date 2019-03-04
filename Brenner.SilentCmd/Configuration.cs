using Brenner.SilentCmd.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Brenner.SilentCmd
{
    public class Configuration
    {
        public bool LogAppend { get; private set; }
        public string LogFilePath { get; private set; }
        public string BatchFilePath { get; set; }
        public string BatchFileArguments { get; private set; }
        public TimeSpan Delay { get; private set; }
        public bool ShowHelp { get; private set; }

        public Configuration()
        {
            LogAppend = Settings.Default.DefaultLogAppend;
            LogFilePath = Settings.Default.DefaultLogFilePath;
            BatchFilePath = Settings.Default.DefaultBatchFilePath;
            BatchFileArguments = Settings.Default.DefaultBatchFileArguments;
            Delay = Settings.Default.DefaultDelay;
        }

        public void ParseArguments(IEnumerable<string> args)
        {
            var argumentsBuilder = new StringBuilder();
            var batchFilePathWasRead = false;
            string argValue;

            foreach (string arg in args)
            {
                if (ArgumentParser.TryGetValue(arg, "/LOG+", out argValue))
                {
                    LogAppend = true;
                    LogFilePath = argValue;
                    continue;
                }

                if (ArgumentParser.TryGetValue(arg, "/LOG", out argValue))
                {
                    LogAppend = false;
                    LogFilePath = argValue;
                    continue;
                }

                if (ArgumentParser.TryGetValue(arg, "/DELAY", out argValue))
                {
                    Delay = TimeSpan.FromSeconds(Convert.ToDouble(argValue));
                    continue;
                }

                if (ArgumentParser.IsName(arg, "/?"))
                {
                    ShowHelp = true;
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
