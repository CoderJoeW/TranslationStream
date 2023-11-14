using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream.OpenAI
{
    internal class OpenAITextToSpeech
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.openai.com/v1/audio/speech";

        public OpenAITextToSpeech()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.Config.OpenAIKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> GetResponseAsync(string text)
        {
            var payload = new
            {
                model = "tts-1",
                input = text,
                voice = "alloy"
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync("speech.mp3", responseBytes);
                return true;
            }

            return false;
        }
    }
}
