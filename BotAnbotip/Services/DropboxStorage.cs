using BotAnbotip.Services.Interfaces;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BotAnbotip.Services
{
    class DropboxStorage : ICloudStorage
    {
        private readonly ILogger<DropboxStorage> _logger;
        private static DropboxClient Client;

        public DropboxStorage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DropboxStorage>();
        }

        public void Authorize(string apiKey)
        {
            try
            {
                Client = new DropboxClient(apiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropbox client authorization error. ApiKey: {apiKey}", apiKey);
                Environment.Exit(1);
            }
        }

        public async Task<bool> UploadAsync(string fileName, string text)
        {
            try
            {
                using (var stream = GenerateStreamFromString(text))
                {
                    await Client.Files.UploadAsync("/" + fileName, WriteMode.Overwrite.Instance, body: stream);
                }
                _logger.LogInformation("File \"{fileName}\" async uploading success", fileName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File \"{fileName}\" async uploading error", fileName);
                return false;
            }

        }

        public async Task<string> DownloadAsync(string fileName)
        {
            try
            {
                var resultStr = "";
                using (var response = await Client.Files.DownloadAsync("/" + fileName))
                {
                    resultStr = await response.GetContentAsStringAsync();
                }
                _logger.LogInformation("File \"{fileName}\" async downloading success", fileName);
                return resultStr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File \"{fileName}\" async downloading error", fileName);
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