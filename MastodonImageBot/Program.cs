using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

#pragma warning disable CS0252

namespace MastodonImageBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");
            if (!Directory.Exists("config") || Directory.GetFiles("config").Length == 0 || args.GetValue(0) == "/new")
            {
                Console.WriteLine("Running initial setup wizard...");
                Initial();
            }
            if (Directory.GetFiles("config").Length >= 2 || args.Length == 0)
            {
                Console.WriteLine("I've detected more than two bot conifgurations, please specify one as an argument.");
                Environment.Exit(0);
            }
            BotConfig conf = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(/*args[0] ?? */Directory.GetFiles("config")[0]));
            Bot.Run(conf);
            System.Threading.Tasks.Task.Delay(-1).GetAwaiter().GetResult();
        }
        #region Initial Setup
        private static void Initial()
        {
            if (!Directory.Exists("config"))
                Directory.CreateDirectory("config");

            JObject cfg = new JObject();
            Console.WriteLine("Initial Setup / Bot Creation Wizard");
            Console.Write("Instance URL for this bot: ");
            cfg["instance"] = Console.ReadLine();
            Console.Write("Bot account's username: ");
            string username = Console.ReadLine();
            cfg["username"] = username;
            Console.Write("Bot account's email: ");
            cfg["email"] = Console.ReadLine();
            Console.Write("Bot account's password: ");
            cfg["password"] = Console.ReadLine(); // heh plaintext passwords
            Console.Write("Danbooru tags to use: ");
            cfg["tags"] = Console.ReadLine();
            Console.Write("Time period (in minutes) to wait between posts: ");
            cfg["interval"] = Console.ReadLine();
            Console.Write("Create a bot with the above settings, and write to config\\bot-" + username + ".json? [y/n]");
            if(Console.ReadKey().Key != ConsoleKey.Y)
            {
                Console.WriteLine("Quitting.");
                return;
            }
            string json = JsonConvert.SerializeObject(cfg);
            File.WriteAllBytes($"config\\bot-{username}.json", System.Text.Encoding.UTF8.GetBytes(json));
            Console.WriteLine("Bot created!");
        }
        #endregion
    }
}
