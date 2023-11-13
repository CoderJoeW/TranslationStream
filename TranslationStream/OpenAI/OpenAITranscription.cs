using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream.OpenAI
{
    internal class OpenAITranscription
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.openai.com/v1/audio/transcriptions";

        public OpenAITranscription()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.API_KEY);
        }

        public async Task<string?> TranscribeAudioAsync(string filePath, string language)
        {
            using var formData = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var streamContent = new StreamContent(fileStream);
            using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());

            formData.Add(fileContent, "file", Path.GetFullPath(filePath));
            formData.Add(new StringContent("whisper-1"), "model");

            var response = await _httpClient.PostAsync(_baseUrl, formData);

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
