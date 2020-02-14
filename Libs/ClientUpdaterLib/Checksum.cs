using System;
using System.IO;
using System.Security.Cryptography;

namespace ClientUpdaterLib
{
    internal static class Checksum
    {
        internal static readonly SHA256Managed Sha256 = new SHA256Managed();
        
        public static string GetFor(string file)
        {
            FileAttributes attr = File.GetAttributes(file);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return "";

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024 * 1024))
                return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
