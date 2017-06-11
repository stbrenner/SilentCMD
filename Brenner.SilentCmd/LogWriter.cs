using System;
using System.Diagnostics;
using System.IO;

namespace Brenner.SilentCmd
{
    internal class LogWriter : IDisposable
    {
        private StreamWriter _writer;

        /// <summary>
        /// Initializies a log writer that logs to the specified path.
        /// </summary>
        /// <param name="logPath">Path to the destination log file.</param>
        /// <param name="append">True if entrie should be added to an existing log file</param>
        public void Initialize(string logPath, bool append = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(logPath))
                {
                    _writer = new StreamWriter(logPath, append);
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
                if (_writer != null)
                {
                    string message = string.Format(format, args);
                    _writer.WriteLine("{0} - {1}", DateTime.Now, message);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}
