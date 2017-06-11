using System;

namespace Brenner.SilentCmd
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            var engine = new Engine();
            return engine.Execute(args);
        }
    }
}
