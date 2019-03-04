using System.Globalization;

namespace Brenner.SilentCmd
{
    public static class ArgumentParser
    {
        public static bool IsName(string arg, string name)
        {
            return arg.StartsWith(name, true, CultureInfo.InvariantCulture) &&
                (name.Length == arg.Length || arg[name.Length] == ':');
        }

        public static bool TryGetValue(string arg, string name, out string value)
        {
            if (IsName(arg, name))
            {
                int startPosition = name.Length + 1;   // +1 because of colon separator
                value = arg.Substring(startPosition).Trim('"');
                return true;
            }

            value = null;
            return false;
        }

    }
}
