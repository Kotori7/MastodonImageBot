using System;
using TootNet;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace MastodonImageBot
{
    public static class Bot
    {
        private static BotConfig Config;
        private static HttpClient http = new HttpClient();
        private static bool running = false;
        public static async void Run(BotConfig conf)
        {
            Config = conf;
            //Console.CancelKeyPress += (s, e) =>
            //{
            //    e.Cancel = true;
            //    running = false;
            //};
            var auth = new Authorize();
            await auth.CreateApp(Config.Instance, "Mastodon image Bot (by Kotori7)", Scope.Write, "https://kotori7.com");
            var tokens = await auth.AuthorizeWithEmail(Config.Email, Config.Password);
            running = true;
            while (running)
            {
                string content = await http.GetStringAsync($"https://danbooru.donmai.us/posts.json?limit=1&random=1&tags={Config.Tags}");
                JArray a = JArray.Parse(content);
                JObject obj = JObject.Parse(a[0].ToString());
                string fPath = $"cache\\{obj["md5"]}.{obj["file_ext"]}";
                string fuck = (string)obj["file_url"];
                if (!fuck.StartsWith("http"))
                {
                    fuck = "https://danbooru.donmai.us" + fuck;
                }
                byte[] fContent = await http.GetByteArrayAsync(fuck);
                File.WriteAllBytes(fPath, fContent);
                using (var fs = new FileStream(fPath, FileMode.Open, FileAccess.Read))
                {
                    var attach = await tokens.Media.PostAsync(file => fs);
                    await tokens.Statuses.PostAsync(status => $"https://danbooru.donmai.us/posts/{obj["id"]}", media_ids => new System.Collections.Generic.List<long>() { attach.Id });
                }
                Console.WriteLine($"Posted danbooru post {obj["id"]}. MD5: {obj["md5"]}");
                await Task.Delay(Config.PostInterval * 60000);
            }
        }
    }
}
