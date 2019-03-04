using System;
using Xunit;

namespace Brenner.SilentCmd.Tests
{
    public class ConfigurationTest
    {
        [Fact]
        public void Log()
        {
            Configuration config = new Configuration();
            config.ParseArguments(new string[] {@"/LOG:c:\temp\test.log"});
            Assert.False(config.LogAppend);
            Assert.Equal(@"c:\temp\test.log", config.LogFilePath);
        }

        [Fact]
        public void NotLog()
        {
            Configuration config = new Configuration();
            config.ParseArguments(new string[] { @"/LOGA:c:\temp\test.log" });
            Assert.Null(config.LogFilePath);
        }

        [Fact]
        public void LogPlus()
        {
            Configuration config = new Configuration();
            config.ParseArguments(new string[] { "/LOG+:\"c:\\My Files\\test.log\"" });
            Assert.True(config.LogAppend);
            Assert.Equal(@"c:\My Files\test.log", config.LogFilePath);
        }

        [Fact]
        public void Delay()
        {
            Configuration config = new Configuration();
            config.ParseArguments(new string[] { "/DELAY:1234" });
            Assert.Equal(TimeSpan.FromSeconds(1234), config.Delay);
        }

        [Fact]
        public void Help()
        {
            Configuration config = new Configuration();
            config.ParseArguments(new string[] { "/?" });
            Assert.True(config.ShowHelp);
        }

    }
}
