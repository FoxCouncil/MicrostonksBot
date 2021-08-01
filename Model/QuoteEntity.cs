// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using Microsoft.Azure.Cosmos.Table;
using System;

namespace MicrostonksBot.Model
{
    public class QuoteEntity : TableEntity
    {
        public QuoteEntity()
        {
        }

        public QuoteEntity(string symbol, Quote quote)
        {
            PartitionKey = Tables.PartitionKey;

            RowKey = Tables.GetStorageRowKey(quote);

            PriceTimestamp = DateTimeOffset.FromUnixTimeSeconds(quote.Timestamp);

            Symbol = symbol;

            PriceOpen = PriceCurrent = quote.PriceCurrent;
        }

        public DateTimeOffset PriceTimestamp { get; set; }

        public string Symbol { get; set; }

        public double PriceOpen { get; set; }

        public double PriceCurrent { get; set; }
        
    }
}
