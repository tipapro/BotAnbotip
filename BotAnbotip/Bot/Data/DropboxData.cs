using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Data
{
    [JsonObject]
    public class DropboxData<T>
    {
        private T _value;
        private readonly string _fileName;

        public T Value { get => _value; set => _value = value;  }
        public DropboxData(string fileName)
        {
            if (fileName != null) _fileName = fileName;
            InitializeVariable();
        }

        public async Task ReadAsync()
        {
            if (!DropboxIntegration.IsAuthorized) DropboxIntegration.Authorize(PrivateData.DropboxApiKey);

            string json = await DropboxIntegration.DownloadAsync(PrivateData.FileNamePrefix + _fileName + ".json");

            if (json != "")
            {
                _value = JsonConvert.DeserializeObject<T>(json);
            }
            else InitializeVariable();
        }

        public async Task SaveAsync(T newValue)
        {
            _value = newValue;
            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            string json = "";
            if (_value != null)
            {
                json = JsonConvert.SerializeObject(_value);               
            }
            await DropboxIntegration.UploadAsync(PrivateData.FileNamePrefix + _fileName + ".json", json);
        }

        private void InitializeVariable()
        {
            try
            {
                var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
                if (constructor == null) _value = default(T);
                else _value = (T)constructor.Invoke(new object[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(_fileName);
                Console.WriteLine(ex.Message);
            }
        }

        public static implicit operator T(DropboxData<T> obj) => obj._value;
    }
}
