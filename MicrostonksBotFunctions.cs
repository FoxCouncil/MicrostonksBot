// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MicrostonksBot
{
    public static class MicrostonksBotFunctions
    {
        [Function("MicrostonksBotTimer")]
        public static async Task RunMicrostonksBotTimer([TimerTrigger("0 */5 * * * *", RunOnStartup = false, UseMonitor = false)] TimerInfo timer, FunctionContext context)
        {
            var logger = context.GetLogger("MicrostonksBotTimer");

            if (!CheckIfNYSEIsOpen())
            {
                logger.LogInformation($"NYSE Markets are not open, will not run.");

                return;
            }

            var stonk = Environment.GetEnvironmentVariable("TheStonk");

            var quote = await Finnhub.GetQuote(stonk);

            var entity = await Tables.GetQuoteEntityFromQuote(quote);

            var randomVersion = false;

            var currentPrice = randomVersion ? entity.PriceOpen + Dice.Roll() : quote.PriceCurrent;

            var difference = currentPrice - entity.PriceOpen;

            if (Math.Abs(difference) >= 1)
            {
                if (difference > 0)
                {
                    await Telegram.SendMessageToSubscribers($"💹 `{stonk}` Stonks ▲ {difference:C} | Current Price: {currentPrice:C}");
                }
                else
                {
                    await Telegram.SendMessageToSubscribers($"📉 `{stonk}` Stonks 🔻 {difference:C} | Current Price: {currentPrice:C}");
                }

                logger.LogInformation($"Enough difference to trigger notification {currentPrice:C} - {entity.PriceOpen:C} = {difference:C}");

                entity.PriceOpen = currentPrice;
            }
            else
            {
                logger.LogInformation($"Not enough difference to trigger notification {currentPrice:C} - {entity.PriceOpen:C} = {difference:C}");
            }

            entity.PriceTimestamp = DateTimeOffset.FromUnixTimeSeconds(quote.Timestamp);
            entity.PriceCurrent = currentPrice;

            await Tables.SetQuoteEntity(entity);
        }

        [Function("MicrostonksBotForceUpdate")]
        public static async Task<HttpResponseData> RunMicrostonksBotForceUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request, FunctionContext context)
        {
            var logger = context.GetLogger("MicrostonksBotForceUpdateWebhook");

            var stonk = Environment.GetEnvironmentVariable("TheStonk");

            var quote = await Finnhub.GetQuote(stonk);

            await Tables.SetQuoteEntityFromQuote(quote);

            logger.LogInformation($"Forced update for stonk {stonk}!");

            return request.CreateResponse(HttpStatusCode.OK);
        }

        [Function("MicrostonksBot")]
        public static async Task<HttpResponseData> RunMicrostonksBot([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request, FunctionContext context)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var logger = context.GetLogger("MicrostonksBotWebhook");

            var body = await request.ReadAsStringAsync();

            var update = JsonConvert.DeserializeObject<Update>(body);

            await Telegram.ProcessUpdate(update);

            logger.LogInformation("Processed message from Telegram.");

            return request.CreateResponse(HttpStatusCode.OK);
        }

        private static bool CheckIfNYSEIsOpen()
        {
            var timeUtc = DateTime.UtcNow;

            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("EST");

            var estNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            if (estNow.DayOfWeek < DayOfWeek.Monday || estNow.DayOfWeek > DayOfWeek.Friday)
            {
                return false;
            }

            if (estNow.Hour < 9 || estNow.Hour > 15)
            {
                return false;
            }

            return true;
        }

        public class TimerInfo
        {
            public MyScheduleStatus ScheduleStatus { get; set; }

            public bool IsPastDue { get; set; }
        }

        public class MyScheduleStatus
        {
            public DateTime Last { get; set; }

            public DateTime Next { get; set; }

            public DateTime LastUpdated { get; set; }
        }
    }
}
