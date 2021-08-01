// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using Microsoft.Azure.Cosmos.Table;
using Telegram.Bot.Types;

namespace MicrostonksBot.Model
{
    class ChatEntity : TableEntity
    {
        public ChatEntity() {}

        public ChatEntity(Chat chat)
        {
            PartitionKey = Tables.PartitionKey;
            RowKey = chat.Id.ToString();
            ChatType = chat.Type.ToString();
            ChatName = Telegram.GetNameFromChat(chat);
            Subscribed = false;
        }

        public string ChatType { get; set; }

        public string ChatName { get; set; }

        public bool Subscribed { get; set; }
    }
}
