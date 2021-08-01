// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using System.Text.Json.Serialization;

namespace MicrostonksBot
{
    public class Quote
    {
        [JsonPropertyName("c")]
        public double PriceCurrent { get; set; }

        [JsonPropertyName("h")]
        public double PriceHigh { get; set; }

        [JsonPropertyName("l")]
        public double PriceLow { get; set; }

        [JsonPropertyName("o")]
        public double PriceOpen { get; set; }

        [JsonPropertyName("pc")]
        public double PricePreviousClose { get; set; }

        [JsonPropertyName("t")]
        public int Timestamp { get; set; }
    }

}
