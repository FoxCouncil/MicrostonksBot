// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using System;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MicrostonksBot
{
    public class Telegram
    {
        internal static async Task ProcessUpdate(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var client = GetClient();

            var msg = update.Message;

            if (msg.Type != MessageType.Text)
            {
                return;
            }

            var chat = msg.Chat;

            var entity = await Tables.GetChat(chat);

            if (msg.Text.StartsWith("/current"))
            {
                var stonk = Environment.GetEnvironmentVariable("TheStonk");

                var singleCurrentQuote = await Tables.GetQuoteEntity();

                if (singleCurrentQuote != null)
                {
                    await SendMessage(msg.Chat.Id, $"📊 `{stonk}` | Current Price: {singleCurrentQuote.PriceCurrent:C}\nLast Updated: {singleCurrentQuote.PriceTimestamp}");
                }
                else
                {
                    await SendMessage(msg.Chat.Id, $"📊 `{stonk}` | Current Price: N/A\nLast Updated: N/A");
                }

                return;
            }
            else if (msg.Text.StartsWith("/version"))
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;

                await SendMessage(msg.Chat.Id, $"MicrostonksBot Version {version} - By Fox - https://github.com/foxcouncil/microstonksbot");

                return;
            }

            // Everything past here, should be commands for ADMINS/CREATORS only!!!

            if (chat.Type != ChatType.Private)
            {
                try
                {
                    var user = await client.GetChatMemberAsync(msg.Chat.Id, msg.From.Id);

                    if (user.Status != ChatMemberStatus.Administrator && user.Status != ChatMemberStatus.Creator)
                    {
                        await SendMessage(msg.Chat.Id, "Nice try...");

                        return;
                    }
                }
                catch (ApiRequestException) { }
            }

            if (msg.Text.StartsWith("/start"))
            {
                if (entity.Subscribed)
                {
                    await SendMessage(msg.Chat.Id, "You are already subscribed! Type /stop to stop updates.");
                }
                else
                {
                    entity.Subscribed = true;

                    await Tables.SetChat(entity);

                    await SendMessage(msg.Chat.Id, "You have been subscribed, type /stop to stop updates.");
                }
            }
            else if (msg.Text.StartsWith("/stop"))
            {
                if (entity.Subscribed)
                {
                    entity.Subscribed = false;

                    await Tables.SetChat(entity);

                    await SendMessage(msg.Chat.Id, "You have been unsubscribed, type /start to resume updates.");
                }
                else
                {
                    await SendMessage(msg.Chat.Id, "You are already unsubscribed! Type /start to resume updates.");
                }
            }

            return;
        }

        public static async Task SendMessage(ChatId id, string msg)
        {
            var client = GetClient();

            await client.SendTextMessageAsync(id, msg, ParseMode.Markdown);
        }

        public static async Task SendMessageToSubscribers(string msg)
        {
            var client = GetClient();

            var chats = await Tables.GetChats();

            foreach (var chat in chats)
            {
                await client.SendTextMessageAsync(chat.RowKey, msg, ParseMode.Markdown);
            }
        }

        internal static string GetNameFromChat(Chat chat)
        {
            return string.IsNullOrWhiteSpace(chat.Username) ? $"[{chat.Title}]" : $"@{chat.Username}";
        }

        private static TelegramBotClient GetClient()
        {
            var token = Environment.GetEnvironmentVariable("TelegrameBotToken");

            return new TelegramBotClient(token);
        }
    }
}
