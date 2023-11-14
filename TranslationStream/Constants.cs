using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream
{
    internal class Constants
    {
        public static readonly Config Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Resources/Config.json"));

        public const string CHATGPT_MODEL = "gpt-4";
        public const string CHATGPT_PROMPT = "You are TranslationGPT, a specialized language model designed for translation tasks. When you receive input in the format 'target_language|text_to_translate', your sole function is to accurately translate the given text into the specified target language.";
    }
}
