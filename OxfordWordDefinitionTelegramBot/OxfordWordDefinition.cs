using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

//using Telegram.Bot;

namespace OxfordWordDefinitionTelegramBot
{
    public static class OxfordWordDefinition
    {
        private const string BotAccessToken = "767656992:AAHw6vUyuDXfds7xx7UtTD3KQGJJG1nOBE0";
        private static readonly ITelegramBotClient BotClient;
        private static readonly ChatId ChannelId = new ChatId(-1001348719212);

        private static string Name;

        static OxfordWordDefinition()
        {
            Name = "OxfordWordDefinition";
            BotClient = new TelegramBotClient(BotAccessToken);
        }

        [FunctionName("OxfordWordDefinition")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            Logger.logger = log;

            Logger.LogInformation(Name, $"function work at: {DateTime.Now}", Logger.Method.Start);

            var oxfordApiUser = new OxfordApiUser();
            var word = await oxfordApiUser.GetNewWord();

            Logger.LogInformation(Name, "sending message", Logger.Method.Start);
            await BotClient.SendTextMessageAsync(ChannelId, word.GenerateMessage());
            Logger.LogInformation(Name, "sending message", Logger.Method.End);

            Logger.LogInformation(Name, $"function work at: {DateTime.Now}", Logger.Method.End);
        }
    }
}
