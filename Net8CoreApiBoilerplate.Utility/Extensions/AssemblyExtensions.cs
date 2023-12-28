using System;
using System.IO;
using System.Reflection;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class AssemblyExtensions
    {
        public static string CurrentAssemblyDirectory()
        {
            // string codeBase = Assembly.GetExecutingAssembly().CodeBase; // leftover from 4.7
            string codeBase = Assembly.GetExecutingAssembly().Location; 
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
