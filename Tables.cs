// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using MicrostonksBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MicrostonksBot
{
    class Tables
    {
        public const string PartitionKey = "microstonksbot";

        private const string kQuoteTableName = "quote";

        private const string kChatTableName = "chats";

        public static async Task SetEntity(string tableName, TableEntity entity)
        {
            var table = await GetCloudTable(tableName);

            var tableMergeOperation = TableOperation.Merge(entity);

            await table.ExecuteAsync(tableMergeOperation);
        }

        public static async Task<List<ChatEntity>> GetChats()
        {
            var table = await GetCloudTable(kChatTableName);

            var query = table.CreateQuery<ChatEntity>().Where(x => x.Subscribed).AsTableQuery();

            return (await query.ExecuteSegmentedAsync(null)).ToList();
        }

        public static async Task<ChatEntity> GetChat(Chat chat)
        {
            var table = await GetCloudTable(kChatTableName);

            var tableOperation = TableOperation.Retrieve<ChatEntity>(PartitionKey, chat.Id.ToString());

            var result = await table.ExecuteAsync(tableOperation);

            if (result.Result == null)
            {
                var entity = new ChatEntity(chat);

                var tableInsertOperation = TableOperation.Insert(entity);

                var insertResult = await table.ExecuteAsync(tableInsertOperation);

                return insertResult.Result as ChatEntity;
            }

            return result.Result as ChatEntity;
        }

        public static async Task SetChat(ChatEntity entity)
        {
            await SetEntity(kChatTableName, entity);
        }

        public static async Task SetQuoteEntity(QuoteEntity entity)
        {
            await SetEntity(kQuoteTableName, entity);
        }

        public static async Task<QuoteEntity> GetQuoteEntity()
        {
            var table = await GetCloudTable(kQuoteTableName);

            var result = table.ExecuteQuery(new TableQuery<QuoteEntity>()).Where(e => e.PartitionKey.Equals(PartitionKey)).OrderByDescending(x => x.PriceTimestamp);

            return result.FirstOrDefault();
        }

        public static async Task<QuoteEntity> GetQuoteEntityFromQuote(Quote quote)
        {
            var table = await GetCloudTable(kQuoteTableName);

            var rowkey = GetStorageRowKey(quote);

            var tableOperation = TableOperation.Retrieve<QuoteEntity>(PartitionKey, rowkey);

            var result = await table.ExecuteAsync(tableOperation);

            if (result.Result == null)
            {
                var insertResult = await SetQuoteEntityFromQuote(quote);

                return insertResult.Result as QuoteEntity;
            }

            return result.Result as QuoteEntity;
        }

        public static async Task<TableResult> SetQuoteEntityFromQuote(Quote quote)
        {
            var stonk = Environment.GetEnvironmentVariable("TheStonk");

            var entity = new QuoteEntity(stonk, quote);

            var tableInsertOperation = TableOperation.Insert(entity);

            var table = await GetCloudTable(kQuoteTableName);

            var insertResult = await table.ExecuteAsync(tableInsertOperation);

            return insertResult;
        }

        public static string GetStorageRowKey(Quote quote)
        {
            var timestamp = DateTimeOffset.FromUnixTimeSeconds(quote.Timestamp);

            return timestamp.Date.ToShortDateString().Replace('/', '-');
        }

        private static async Task<CloudTable> GetCloudTable(string tablename)
        {
            var storageAccountString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var cloudStorage = CloudStorageAccount.Parse(storageAccountString);

            var tableClient = cloudStorage.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tablename);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
