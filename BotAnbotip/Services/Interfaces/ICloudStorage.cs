using System.Threading.Tasks;

namespace BotAnbotip.Services.Interfaces
{
    interface ICloudStorage
    {
        void Authorize(string apiKey);

        Task<string> DownloadAsync(string fileName);
        Task<bool> UploadAsync(string fileName, string text);
    }
}
