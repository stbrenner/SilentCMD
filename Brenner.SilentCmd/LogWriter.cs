using System;
using System.Diagnostics;
using System.IO;

namespace Brenner.SilentCmd
{
    internal class LogWriter : IDisposable
    {
        private string _fullPath;
        private StreamWriter _writer;
        private long _maxSize;
        private bool _append;

        public bool Initialized { get { return _writer != null; } }

        /// <summary>
        /// Initializies a log writer that logs to the specified path.
        /// </summary>
        /// <param name="logPath">Path to the destination log file.</param>
        /// <param name="append">True if entrie should be added to an existing log file</param>
        public void Initialize(string logPath = null, bool append = false, long maxSize = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(logPath)) return;   // No logging if no path specified
                _fullPath = Environment.ExpandEnvironmentVariables(logPath);

                _append = append;
                _maxSize = maxSize;

                if (_writer != null) _writer.Dispose();
                _writer = new StreamWriter(_fullPath, _append);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }            
        }

        private void RotateLogFile()
        {
            try
            {
                if (_maxSize <= 0) return;   // Ignore if no max size specified

                if (_writer != null && _writer.BaseStream != null && _writer.BaseStream.Length > _maxSize)
                {
                    _writer.Dispose();
                    File.Copy(_fullPath, _fullPath + ".old", true);
                    File.Delete(_fullPath);
                    _writer = new StreamWriter(_fullPath, _append);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public void Dispose()
        {
            try
            {
                if (_writer != null)
                {
                    _writer.Dispose();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            GC.SuppressFinalize(this);
        }

        public void WriteLine(string format, params object[] args)
        {
            try
            {
                RotateLogFile();

                if (_writer != null && format != null)
                {
                    string message = string.Format(format, args);
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    _writer.WriteLine("{0} - {1}", timestamp, message);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}
