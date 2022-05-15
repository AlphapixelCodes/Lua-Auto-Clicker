using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLUA_Test
{
    public static class StorageManager
    {
        public static string ResourcesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LuaClicker";
        public static bool StorageExistedAtStart, DeleteLocalFilesOnExit;

        public static void InitStorage()
        {
            if (!Directory.Exists(ResourcesPath))
            {
                StorageExistedAtStart = false;
                Directory.CreateDirectory(ResourcesPath);
            }
            else
            {
                StorageExistedAtStart = true;
            }
        }
        public static void BrowseLocalFiles()
        {
            if (Directory.Exists(ResourcesPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = ResourcesPath,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
        }

        internal static string getFilePath(string v)
        {
            return ResourcesPath + "\\" + v;
        }

        public static void SaveFile(string relativePath, string data)
        {
            File.WriteAllText(getFilePath(relativePath), data);
        }
        public static bool FileExists(string relativePath)
        {
            return File.Exists(getFilePath(relativePath));
        }
    }
}
