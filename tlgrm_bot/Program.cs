using System;
using System.Collections.Generic;
using AIMLbot;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using System.Net;
using System.Threading;

namespace AIMLTelegramBot
{
    class Program
    {
        private static ITelegramBotClient botClient;
        private static Bot AI;
        private static Dictionary<long, User> users;
        private static string LoadKey(string pathToFile)
        {
            StreamReader sr = new StreamReader(pathToFile);
            string key = sr.ReadLine().Replace(" ", string.Empty);

            return key;
        }
        private static async void actionOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Входящее сообщение в чате: {e.Message.Chat.Id}");
                Console.WriteLine($"Входящее сообщение: {e.Message.Text}");

                AI.isAcceptingUserInput = false;
                User currentUser;
                if (users.ContainsKey(e.Message.Chat.Id))
                    currentUser = users[e.Message.Chat.Id];
                else
                {
                    currentUser = new User(e.Message.Chat.Id.ToString(), AI);
                    users.Add(e.Message.Chat.Id, currentUser);
                }
                AI.isAcceptingUserInput = true;
                Request r = new Request(e.Message.Text, currentUser, AI);
                Result res = AI.Chat(r);

                Console.WriteLine("Robot: " + res.Output);
                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Robot:\n" + res.Output
                );
            }
        }
        static void Main(string[] args)
        {
            AI = new Bot();
            AI.loadSettings();
            AI.loadAIMLFromFiles();
            users = new Dictionary<long, User>();

            botClient = new TelegramBotClient(access token);
            var me = botClient.GetMeAsync().Result;

            Console.WriteLine(
                $"I'M ALIVE {me.Id} :: {me.FirstName}"
            );

            botClient.OnMessage += actionOnMessage;
            botClient.StartReceiving();

            Thread.Sleep(int.MaxValue);
        }
    }
}
