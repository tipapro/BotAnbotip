using BotAnbotip.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BotAnbotip.Data
{
    [JsonObject]
    public class CloudData<T>
    {
        private readonly string _fileName;

        public T Value { get; set; }

        public CloudData(string fileName)
        {
            _fileName = fileName;
        }

        public async Task ReadAsync()
        {
            string json = await ServiceControlManager.CloudStorage.DownloadAsync(PrivateData.FileNamePrefix + _fileName + ".json");

            if (json != "") Value = JsonConvert.DeserializeObject<T>(json);
            else Initialize();
        }

        public async Task SaveAsync(T newValue)
        {
            Value = newValue;
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            string json = "";
            if (Value != null)
            {
                json = JsonConvert.SerializeObject(Value);               
            }
            await ServiceControlManager.CloudStorage.UploadAsync(PrivateData.FileNamePrefix + _fileName + ".json", json);
        }

        private void Initialize()
        {
            var type = typeof(T);
            Value = type.IsValueType ? default(T) : (T)type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]);
        }

        public static implicit operator T(CloudData<T> obj) => obj.Value;
    }
}
