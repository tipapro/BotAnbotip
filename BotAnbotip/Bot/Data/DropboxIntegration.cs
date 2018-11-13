using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BotAnbotip.Bot.Data
{
    class DropboxIntegration
    {
        private static DropboxClient Client;
        public static bool IsAuthorized;

        public static void Authorize(string apiKey)
        {
            Client = new DropboxClient(apiKey);
            IsAuthorized = true;
        }

        public static async Task<bool> UploadAsync(string uploadFileName, string text)
        {
            if (!IsAuthorized) Authorize(PrivateData.DropboxApiKey);
            try
            {
                using (var stream = GenerateStreamFromString(text))
                {
                    await Client.Files.UploadAsync("/" + uploadFileName, WriteMode.Overwrite.Instance, body: stream);
                }
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("UploadAsync Error: " + uploadFileName);
                return false;
            }

        }

        public static async Task<string> DownloadAsync(string DropboxFileName)
        {
            if (!IsAuthorized) Authorize(PrivateData.DropboxApiKey);
            try
            {
                var response = await Client.Files.DownloadAsync("/" + DropboxFileName);
                string result = await response.GetContentAsStringAsync();
                Console.WriteLine("DownloadAsync Success: " + DropboxFileName);
                return result;
            }
            catch (Exception)
            {
                Console.WriteLine("DownloadAsync Error: " + DropboxFileName);
                return string.Empty;
            }

        }

        public static Stream GenerateStreamFromString(string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}