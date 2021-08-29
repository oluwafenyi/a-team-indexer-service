using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SearchifyEngine.Indexer
{
    public class ExtractDoc {
        private static string folder = "/tmp/";
        public static string Extract(string url)
        {
            try
            {
                
                if (!Directory.Exists(Path.GetFullPath(folder))){
                    Console.WriteLine("created directory");

                    Directory.CreateDirectory(Path.GetFullPath(folder));
                }

                if (File.Exists(Path.Combine(Path.GetFullPath(folder), url)))
                {
                    File.Delete(url);
                }
                using (var client = new WebClient())
                {
                    var innerUrl = new Uri(url);
                    var hashString = GetHashString(url);
                    client.Credentials = new NetworkCredential("UserName", "Password");
                    client.DownloadFile(innerUrl, Path.Combine(Path.GetFullPath(folder), hashString));
                    return Path.Combine(Path.GetFullPath(folder), hashString);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e + " Error: while downloading file");
            }

            return null;
        }

        public static void Delete(string path)
        {
            if (File.Exists(Path.Combine(Path.GetFullPath(folder), path)))
            {
                File.Delete(path);
            }
        }


        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}