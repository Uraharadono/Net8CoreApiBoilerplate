using System;
using System.Diagnostics;
using System.IO;
using Net8CoreApiBoilerplate.Infrastructure.Settings;

namespace Net8CoreApiBoilerplate.Api.Utility
{
    public static class FolderBuilder
    {
        public static void BuildFolders(IAppSettings settings)
        {
            try
            {
                // Base folder
                if (Directory.Exists(settings.BaseFolder) == false)
                {
                    Directory.CreateDirectory(settings.BaseFolder);
                }

                // Now map children - only if they have suffix "Folder" in the name
                foreach (System.Reflection.PropertyInfo p in settings.GetType().GetProperties())
                {
                    if (p.CanRead)
                    {
                        if (p.Name.Contains("Folder") && !p.Name.Contains("BaseFolder"))
                        {
                            var fullPath = $"{settings.BaseFolder}{p.GetValue(settings, null)}";
                            if (Directory.Exists(fullPath) == false)
                            {
                                Directory.CreateDirectory(fullPath);
                            }
                        }
                        Debug.WriteLine("{0}: {1}", p.Name, p.GetValue(settings, null)); //possible function
                    }
                }

            }
            catch (Exception e)
            {
                // Silent fail, but I can't make my app not even run if folders are not created.
                Console.WriteLine(e);
            }
        }
    }
}
