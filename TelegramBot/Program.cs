using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var botClient = new TelegramBotClient(System.IO.File.ReadAllText(@"E:\\Documents\\projects\\TelegramBot\\token.txt"));

        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine("Bot started!");
        Console.ReadLine();

        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var username = message.Chat.Username;
            var receivedate = message.Date;

            Console.WriteLine($"Received a '{message.Text}' message in chat {chatId} from {username} at {receivedate}.");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
            new KeyboardButton[] { "Привет", "Шакалы" },
            })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите ответ",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);

            if (message.Text.ToLower().Contains("здарова") || message.Text.ToLower().Contains("привет"))
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Здравствуйте, я бот по имени Sharpy! Меня так завут, потому-что я написан на языке программирования C#!",
                parseMode: ParseMode.Html,
                disableNotification: true,
                replyToMessageId: update.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl(
                        text: "Вокуев прогульщик",
                        url: "https://t.me/poncheg11")),
                cancellationToken: cancellationToken);
            }

            if (message.Text.ToLower().Contains("шакалы"))
            {
                await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/8/82/Canis_aureus_-_golden_jackal.jpg/275px-Canis_aureus_-_golden_jackal.jpg",
                caption: "<b>Шакалы</b>. <i>Источник:</i>: <a href=\"https://ru.wikipedia.org/wiki/Обыкновенный_шакал\">Wikipedia</a>",
                parseMode: ParseMode.Html,
                disableNotification: true,
                replyToMessageId: update.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl(
                        text: "Вокуев прогульщик",
                        url: "https://t.me/poncheg11")),
                cancellationToken: cancellationToken);
            }

        }



        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}