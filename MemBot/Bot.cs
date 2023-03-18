using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace MemBot
{
    public class Bot
    {
        public Bot(IConfiguration configuration, TelegramBotClient botClient)
        {
            _configuration = configuration;
            _botClient = botClient;
        }

        private readonly IConfiguration _configuration;
        private TelegramBotClient _botClient;

        public async Task<TelegramBotClient> GetClient()
        {
            _botClient = new TelegramBotClient(_configuration["Token"]);
            var hook = _configuration["Url"];

            //await _botClient.SetWebhookAsync(hook, dropPendingUpdates: true);
            await _botClient.SetWebhookAsync(hook);
            return _botClient;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}