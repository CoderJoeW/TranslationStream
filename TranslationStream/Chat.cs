using NAudio.Wave;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationStream.Models;
using TranslationStream.OpenAI;

namespace TranslationStream
{
    internal class Chat
    {
        public string? Username {  get; set; }
        public string? Language { get; set; }
        public RedisChannel Channel { get; set; }

        private ChatGPTClient _chatGpt;
        private OpenAITextToSpeech _textToSpeech;
        private OpenAITranscription _textTranscription;

        private AudioRecording _audioRecording;

        public Chat()
        {
            _chatGpt = new ChatGPTClient();
            _textToSpeech = new OpenAITextToSpeech();
            _textTranscription = new OpenAITranscription();
            _audioRecording = new AudioRecording();
        }

        public void GetUsername()
        {
            Console.WriteLine("Please enter your name:");
            Username = Console.ReadLine();
            while (string.IsNullOrEmpty(Username))
            {
                Console.WriteLine("Please enter a valid name:");
                Username = Console.ReadLine();
            }
        }

        public void GetLanguage()
        {
            Console.WriteLine("Please type your preferred language(default English):");
            Language = Console.ReadLine();

            if(string.IsNullOrEmpty(Language))
            {
                Language = "English";
            }
        }

        public void GetChannel()
        {
            Console.WriteLine("Please enter channel you would like to join:");
            string? channelName = Console.ReadLine();
            while(string.IsNullOrEmpty(channelName))
            {
                Console.WriteLine("Please enter a valid channel name:");
                channelName = Console.ReadLine();
            }

            Channel = RedisChannel.Literal(channelName);
        }

        public void SubscribeToChannel()
        {
            RedisManager.Instance.Subscriber.Subscribe(Channel, async (channel, message) =>
            {
                string[] messageParts = message.ToString().Split('~');
                string user = messageParts[0];
                string content = messageParts[1];

                if (user == Username) return;

                var response = await _chatGpt.GetResponseAsync(Constants.CHATGPT_MODEL, Constants.CHATGPT_PROMPT, $"{Language}|{content}");

                if(response == null) return;

                ChatGPTResponse? chatGPTResponse = JsonConvert.DeserializeObject<ChatGPTResponse>(response);

                if(chatGPTResponse == null) return;

                Stopwatch watch = Stopwatch.StartNew();
                bool convertTextToSpeech = await _textToSpeech.GetResponseAsync(chatGPTResponse.choices[0].message.content);
                watch.Stop();
                Console.WriteLine($"Timings: {watch.ElapsedMilliseconds}");

                if (!convertTextToSpeech) return;

                Mp3Player.ConvertMp3ToWav("speech.mp3", "speech.wav");
                Mp3Player.PlayWavFile("speech.wav");

                Console.WriteLine(chatGPTResponse.choices[0].message.content);
            });
        }

        public async void ChatLoop()
        {
            Console.WriteLine("Press and hold the spacebar to record audio. Release to stop recording.");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if(keyInfo.Key == ConsoleKey.Spacebar)
                    {
                        if (!_audioRecording.IsRecording)
                        {
                            _audioRecording.StartRecording();
                        }
                        else
                        {
                            if (_audioRecording.IsRecording)
                            {
                                _audioRecording.StopRecording();

                                while(_audioRecording.IsRecording)
                                {
                                    Console.WriteLine("Waiting for audio to finish recording");
                                }

                                if (!string.IsNullOrEmpty(Language))
                                {
                                    string? input = await _textTranscription.TranscribeAudioAsync("audio.wav", Language);

                                    if (input != null)
                                    {
                                        Console.WriteLine(input);

                                        OpenAITranscriptionResponse? res = JsonConvert.DeserializeObject<OpenAITranscriptionResponse>(input);

                                        if (res != null)
                                        {
                                            RedisManager.Instance.Subscriber.Publish(Channel, $"{Username}~{res.text}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("input was null");
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
            
            
            //string? input = Console.ReadLine();
            //while (string.IsNullOrEmpty(input))
            //{
            //    Console.WriteLine("We did not capture any input please retry:");
            //    input = Console.ReadLine();
            //}
            //
            //RedisManager.Instance.Subscriber.Publish(Channel, $"{Username}~{input}");
            //
            //ChatLoop();
        }
    }
}
