## MicrostonksBot
It makes the stonks show up on Telegram. Made by [Fox](https://foxcouncil.com).

#### Name?

| Micro | Stonks | Bot |
| - | - | --|
| [One] | [Stocks] | [Bot] |

## Change Log
- #### Version - 1.0.0.0
  - Basic stuff
  - Only NYSE (New York Stock Exchange)
  - Alerts if stonk changes by `$1`
  - Defaults to Microsoft Stock

## Roadmap
- Graphs
- Daily Summaries
- Memes
- Animated Stuff
- More Stonks Support
- Global Stonks Support
- Portfolio
- Space Travel

## Build Instructions
1. Copy `local-example.settings.json` to `local.settings.json`
2. Replace `TheStonk` with the stock ticker of your choice.
3. Replace `TelegramBotToken` with a token from a bot from https://t.me/botfather/
4. Replace `FinnhubApiKey` with an API key from https://finnhub.io/, c'mon it's free!
5. Set TelegramBotAPI Webhook by sending a request like so: https://api.telegram.org/bot{{botToken}}/setWebhook?url=http://localhost/api/MicrostonksBot
   1. Use https://ngrok.com/ if you need to tunnel the webhook to your local bot.

