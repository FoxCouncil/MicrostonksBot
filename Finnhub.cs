// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicrostonksBot
{
    public static class Finnhub
    {
        private static readonly HttpClient _webClient = new();

        public static async Task<Quote> GetQuote(string symbol)
        {
            SetupClient();

            var rawJson = await _webClient.GetStreamAsync($"https://finnhub.io/api/v1/quote?symbol={symbol}");

            return await JsonSerializer.DeserializeAsync<Quote>(rawJson);
        }

        private static void SetupClient()
        {
            var apiKey = Environment.GetEnvironmentVariable("FinnhubApiKey");

            _webClient.DefaultRequestHeaders.Clear();
            _webClient.DefaultRequestHeaders.Add("X-Finnhub-Token", apiKey);
        }
    }
}
