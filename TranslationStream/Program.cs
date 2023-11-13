using Newtonsoft.Json;
using StackExchange.Redis;
using TranslationStream;
using TranslationStream.Models;

Thread t = new Thread(() => ConsoleLoop());
t.Start();

Chat chat = new Chat();

chat.GetUsername();
chat.GetLanguage();
chat.GetChannel();
chat.SubscribeToChannel();
chat.ChatLoop();

void ConsoleLoop()
{
    SpinWait.SpinUntil(() => false);
}