using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Brenner.SilentCmd.Properties;
using System.IO;
using System.Linq;

namespace Brenner.SilentCmd
{
    internal class Engine
    {
        private string _batchFilePath = Settings.Default.DefaultBatchFilePath;
        private string _batchFileArguments = Settings.Default.DefaultBatchFileArguments;
        private string _logFilePath = Settings.Default.DefaultLogFilePath;
        private bool _logAppend = Settings.Default.DefaultLogAppend;
        private readonly LogWriter _logWriter = new LogWriter();
        
        /// <summary>
        /// Executes the batch file defined in the arguments
        /// </summary>
        public int Execute(string[] args)
        {
            try
            {
                ParseArguments(args);

                if (string.IsNullOrEmpty(_batchFilePath))
                {
                    ShowHelp();
                    return 0;
                }

                ResolveBatchFilePath();

                _logWriter.Initialize(_logFilePath, _logAppend);
                _logWriter.WriteLine(Resources.StartingCommand, _batchFilePath);

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(_batchFilePath, _batchFileArguments)
                                            {
                                                RedirectStandardOutput = true,
                                                RedirectStandardError = true,
                                                UseShellExecute = false,   // CreateNoWindow only works, if shell is not used
                                                CreateNoWindow = true
                                            };
                    process.OutputDataReceived += OutputHandler;
                    process.ErrorDataReceived += OutputHandler;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            catch (Exception e)
            {
                _logWriter.WriteLine(Resources.Error, e.Message);
                return 1;
            }
            finally
            {
                _logWriter.WriteLine(Resources.FinishedCommand, _batchFilePath);
                _logWriter.Dispose();                
            }
        }

        private static void ShowHelp()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            string userManual = string.Format(Resources.UserManual, name.Version);
            MessageBox.Show(userManual, Resources.ProgramTitle);
        }

        private void ResolveBatchFilePath()
        {
            if (string.IsNullOrEmpty(_batchFilePath)) return;

            if (!string.IsNullOrEmpty(Path.GetDirectoryName(_batchFilePath))) return;

            if (string.IsNullOrEmpty(Path.GetExtension(_batchFilePath)))
            {
                if (FindPath(_batchFilePath + ".bat")) return;
                FindPath(_batchFilePath + ".cmd");
            }
            else
            {
                FindPath(_batchFilePath);
            }
        }

        /// <returns>True if file was found</returns>
        private bool FindPath(string filename)
        {
            string currentPath = Path.Combine(Environment.CurrentDirectory, filename);
            if (File.Exists(currentPath)) return true;

            var enviromentPath = System.Environment.GetEnvironmentVariable("PATH");

            var paths = enviromentPath.Split(';');
            var fullPath = paths.Select(x => Path.Combine(x, filename))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();

            if (!string.IsNullOrEmpty(fullPath))
            {
                _batchFilePath = fullPath;
                return true;
            }

            return false;
        }

        private void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            _logWriter.WriteLine(e.Data);
        }

        private void ParseArguments(IEnumerable<string> args)
        {
            var argumentsBuilder = new StringBuilder();
            var batchFilePathWasRead = false;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/LOG:", true, CultureInfo.InvariantCulture))
                {
                    _logAppend = false;
                    _logFilePath = arg.Substring(5).Trim('"');
                    continue;
                }

                if (arg.StartsWith("/LOG+:", true, CultureInfo.InvariantCulture))
                {
                    _logAppend = true;
                    _logFilePath = arg.Substring(6).Trim('"');
                    continue;
                }

                if (!batchFilePathWasRead)
                {
                    _batchFilePath = arg;
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
                _batchFileArguments = argumentsBuilder.ToString();
            }
        }
    }
}
