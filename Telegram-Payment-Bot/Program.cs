using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;
using System.Threading;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

public class Program
{
    private static string SELLIX_API_KEY = "6455238621:AAEWpyzS7hB1LOz4yNGddn7tARmsEmU8xWg";
    private static string TOKEN = "6455238621:AAEWpyzS7hB1LOz4yNGddn7tARmsEmU8xWg";

    private static SQLiteDBManager db_manager = new SQLiteDBManager("order_status.sqlite3");

    // Define the indexes based on your database schema
    private static int UNIQID_INDEX = 0;
    private static int CHAT_ID_INDEX = 1;
    private static int USER_ID_INDEX = 2;
    private static int USERNAME_INDEX = 3;
    private static int STATUS_INDEX = 4;
    private static int CRYPTO_INDEX = 5;
    private static int AMOUNT_INDEX = 6;
    private static int PLAN_INDEX = 7;
    private static int HASH_INDEX = 8;

    static void StartPeriodicCheck(CallbackContext context)
    {
        Dictionary<string, object> jobContext = new Dictionary<string, object>();
        object rawContext = context.Job.Context;
        if (rawContext is Dictionary<string, object>)
        {
            jobContext = (Dictionary<string, object>)rawContext;
        }
        else if (rawContext is IEnumerable<KeyValuePair<string, object>>)
        {
            jobContext = new Dictionary<string, object>((IEnumerable<KeyValuePair<string, object>>)rawContext);
        }
        else
        {
            Console.Error.WriteLine("Unexpected job context format. Ensure it's a dictionary or an iterable of key-value pairs.");
            return;
        }

        long chatId = (long)jobContext.GetValueOrDefault("chat_id");
        string uniqid = (string)jobContext.GetValueOrDefault("uniqid");

        if (chatId == 0 || string.IsNullOrEmpty(uniqid))
        {
            Console.Error.WriteLine("Chat ID or Uniqid missing from the job context.");
            return;
        }

        DateTime now = DateTime.Now;

        DateTime firstCheckTime = (DateTime)jobContext.GetValueOrDefault("first_check_time", now);
        jobContext["first_check_time"] = firstCheckTime;

        TimeSpan timeDiff = now - firstCheckTime;

        TimeSpan deleteAfter = TimeSpan.FromHours(2);

        (bool currentStatus, string cryptoHash) = CheckOrderStatus(SELLIX_API_KEY, uniqid);

        if (currentStatus)
        {
            string lastStatus = db_manager.GetOrderStatus(uniqid);

            List<(string, string)> validTransitions = new List<(string, string)>
            {
                ("PENDING", "WAITING_FOR_CONFIRMATIONS"),
                ("PENDING", "COMPLETED"),
                ("WAITING_FOR_CONFIRMATIONS", "COMPLETED")
            };

            if (validTransitions.Contains((lastStatus, currentStatus)))
            {
                string message = $"Order <code>{uniqid}</code> status changed from <code>{lastStatus}</code> to <code>{currentStatus}</code>";
                context.Bot.SendMessage(chatId: chatId, text: message, parseMode: ParseMode.Html);
                Console.WriteLine($"Order {uniqid} status changed from {lastStatus} to {currentStatus}");
                db_manager.UpdateOrderStatus(uniqid, currentStatus);
            }
        }
    }

    static (bool, string) CheckOrderStatus(string apiKey, string uniqid)
    {
        // Implement the CheckOrderStatus function here
        throw new NotImplementedException();
    }
}

public class SQLiteDBManager
{
    private string _connectionString;

    public SQLiteDBManager(string databaseFile)
    {
        _connectionString = $"Data Source={databaseFile};";
    }

    public string GetOrderStatus(string uniqid)
    {
        // Implement the GetOrderStatus function here
        throw new NotImplementedException();
    }

    public void UpdateOrderStatus(string uniqid, string status)
    {
        // Implement the UpdateOrderStatus function here
        throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class OrderManager
    {
        private readonly DbManager _dbManager;
        private readonly TelegramBotClient _botClient;
        private readonly string _sellixApiKey;
        private readonly int _deleteAfter;

        public OrderManager(DbManager dbManager, TelegramBotClient botClient, string sellixApiKey, int deleteAfter)
        {
            _dbManager = dbManager;
            _botClient = botClient;
            _sellixApiKey = sellixApiKey;
            _deleteAfter = deleteAfter;
        }

        public async Task ProcessOrder(string uniqid, string cryptoHash, long chatId, string lastStatus, TimeSpan timeDiff, string currentStatus)
        {
            var orderDetails = await _dbManager.GetOrderDetails(uniqid);
            if (cryptoHash != null && orderDetails != null && orderDetails[DbManager.HashIndex] != cryptoHash)
            {
                await _dbManager.UpdateOrderHash(uniqid, cryptoHash);
                await _botClient.SendMessageAsync(chatId, $"Transaction hash: <code>{cryptoHash}</code>", ParseMode.Html);
            }

            if (lastStatus == "PENDING" && timeDiff > TimeSpan.FromSeconds(_deleteAfter))
            {
                var (success, message) = await DeleteSellixOrder(_sellixApiKey, uniqid);
                if (success)
                {
                    await _botClient.SendMessageAsync(chatId, $"Order <code>{uniqid}</code> has been automatically cancelled due to timeout.", ParseMode.Html);
                    await _dbManager.UpdateOrderStatus(uniqid, "CANCELLED");
                    _logger.LogInformation($"Order {uniqid} has been automatically cancelled due to timeout, Placed by: {chatId}");
                    context.Job.ScheduleRemoval();
                }
                else
                {
                    _logger.LogError($"Failed to automatically cancel order {uniqid}: {message}");
                }
            }

            if (currentStatus == "COMPLETED")
            {
                context.Job.Enabled = false;
                _logger.LogInformation($"Order {uniqid} is successfully completed, Buyer: {chatId}, Removing the Job");
                context.Job.ScheduleRemoval();
            }
            else if (currentStatus == "VOIDED")
            {
                context.Job.Enabled = false;
                _logger.LogInformation($"Order {uniqid} Was Cancelled, Buyer: {chatId}, Removing the Job");
                context.Job.ScheduleRemoval();
            }
            else
            {
                _logger.LogError($"No current status for order {uniqid}. It might be an API error or network issue.");
            }
        }

        private async Task<(bool, string)> DeleteSellixOrder(string sellixApiKey, string uniqid)
        {
            // Implement the logic to delete the Sellix order using the provided API key and order ID
            throw new NotImplementedException();
        }
    }

    public class DbManager
    {
        public const int HashIndex = 0;

        public async Task<Dictionary<int, string>> GetOrderDetails(string uniqid)
        {
            // Implement the logic to retrieve order details from the database
            throw new NotImplementedException();
        }

        public async Task UpdateOrderHash(string uniqid, string cryptoHash)
        {
            // Implement the logic to update the order hash in the database
            throw new NotImplementedException();
        }

        public async Task UpdateOrderStatus(string uniqid, string status)
        {
            // Implement the logic to update the order status in the database
            throw new NotImplementedException();
        }
    }
}

using Telegram.Bot.Types;
using Telegram.Bot.Args;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

public static void status(Update update, CallbackContext context)
{
    string username = update.EffectiveUser.Username;
    long userid = update.EffectiveUser.Id;
    string[] args = context.Args;
    if (args.Length == 1)
    {
        string uniqid = args[0];
        string order_status = db_manager.get_order_status(uniqid);
        if (order_status != null)
        {
            update.Message.ReplyText($"Current status of order <code>{uniqid}</code> is <code>{order_status}</code>", parseMode: "HTML");
            logging.Info($"{username} - {userid}: Executed /status, Current status of order {uniqid} is {order_status}");
        }
        else
        {
            update.Message.ReplyText("Unable to fetch the status. Please check the uniqid and try again.");
            logging.Error($"{username} - {userid}: Executed /status, Unable to fetch the status. Please check the uniqid and try again.");
        }
    }
    else
    {
        update.Message.ReplyText("Usage: /status <uniqid>");
    }
}

public static void cancel(Update update, CallbackContext context)
{
    string[] args = context.Args;
    if (args.Length == 1)
    {
        string uniqid = args[0];
        long current_chat_id = update.EffectiveChat.Id;
        long current_user_id = update.EffectiveUser.Id;
        string username = update.EffectiveUser.Username;
    }
}

using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

if (order_details != null && order_details[CHAT_ID_INDEX] == current_chat_id && order_details[USER_ID_INDEX] == current_user_id)
{
    if (order_details[STATUS_INDEX].ToUpper() == "PENDING")
    {
        (bool success, string message) = DeleteSellixOrder(SELLIX_API_KEY, uniqid);
        if (success)
        {
            db_manager.UpdateOrderStatus(uniqid, "Cancelled");
            update.Message.ReplyText($"Order <code>{uniqid}</code> has been cancelled successfully.", ParseMode.Html);
            logging.Info($"{username} - {current_user_id} Executed /Cancel, Order {uniqid} has been cancelled successfully.");
        }
        else
        {
            update.Message.ReplyText($"Failed to cancel order <code>{uniqid}</code>: {message}", ParseMode.Html);
            logging.Error($"{username} - {current_user_id} Failed to cancel order {uniqid}: {message}");
        }
    }
    else if (order_details[STATUS_INDEX].ToUpper() == "CANCELLED")
    {
        update.Message.ReplyText($"Order <code>{uniqid}</code> is already cancelled.", ParseMode.Html);
    }
    else
    {
        update.Message.ReplyText("The order cannot be cancelled.");
    }
}
else
{
    update.Message.ReplyText("You do not have permission to cancel this order");
    logging.Info($"{username} - {current_user_id} Tried to Cancel an order they did not place.");
}
else
{
    update.Message.ReplyText("Usage: /cancel <Order ID>");
}

void Start(Update update, CallbackContext context)
{
    string username = update.EffectiveUser.Username;
    long userid = update.EffectiveUser.Id;
    string welcome_message = $"Welcome @{username} to <name placeholder> Bot!";
    logging.Info($"{username} - {userid} Pressed /start");

    List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>
    {
        new List<InlineKeyboardButton> { new InlineKeyboardButton("License", "license") },
        new List<InlineKeyboardButton> { new InlineKeyboardButton("Generic 1", "generic1") },
        new List<InlineKeyboardButton> { new InlineKeyboardButton("Generic 2", "generic2") },
        new List<InlineKeyboardButton> { new InlineKeyboardButton("Generic 3", "generic3") }
    };
    InlineKeyboardMarkup reply_markup = new InlineKeyboardMarkup(keyboard);
    update.Message.ReplyText(welcome_message, replyMarkup: reply_markup);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

public static void Button(Update update, CallbackContext context)
{
    var query = update.CallbackQuery;
    query.Answer();

    var originalMessage = query.Message;
    var chatId = update.EffectiveChat.Id;
    var userId = update.EffectiveUser.Id;
    var username = update.EffectiveUser.Username;

    if (query.Data == "license")
    {
        _logger.LogInformation($"{username} - {userId} Pressed License button.");
        var keyboard = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Buy new License", "buy_new_license") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Renew License (Not Developed)", "renew_license") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Activate License (Not Developed)", "renew_license") }
        };
        var replyMarkup = new InlineKeyboardMarkup(keyboard);
        originalMessage.EditText("Please choose an option:", replyMarkup: replyMarkup);
    }
    else if (query.Data == "buy_new_license")
    {
        _logger.LogInformation($"{username} - {userId} Pressed Buy License button.");
        var keyboard = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Basic Plan: $599", "basicplan") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Gold Plan: $999", "goldplan") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Diamond Plan: $1,499", "diamondplan") }
        };
        var replyMarkup = new InlineKeyboardMarkup(keyboard);
        originalMessage.EditText("Choose your plan:", replyMarkup: replyMarkup);
    }
    else if (new[] { "basicplan", "goldplan", "diamondplan" }.Contains(query.Data))
    {
        var selectedPlan = query.Data;
        _logger.LogInformation($"{username} - {userId} Selected {selectedPlan}.");
        var keyboard = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Bitcoin", $"{selectedPlan}_Bitcoin") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Litecoin", $"{selectedPlan}_Litecoin") },
            new List<InlineKeyboardButton> { new InlineKeyboardButton("Ethereum", $"{selectedPlan}_Ethereum") }
        };
        var replyMarkup = new InlineKeyboardMarkup(keyboard);
        originalMessage.EditText("Choose the cryptocurrency you want to use for payment:", replyMarkup: replyMarkup);
    }
    else if (new[]
    {
        "basicplan_Bitcoin", "basicplan_Litecoin", "basicplan_Ethereum",
        "goldplan_Bitcoin", "goldplan_Litecoin", "goldplan_Ethereum",
        "diamondplan_Bitcoin", "diamondplan_Litecoin", "diamondplan_Ethereum"
    }.Contains(query.Data))
    {
        originalMessage.EditText("Generating Invoice...");
        var parts = query.Data.Split('_');
        var selectedPlan = parts[0];
        var gateway = parts[1].ToUpper();
        var flag = gateway switch
        {
            "BITCOIN" => "BTC",
            "LITECOIN" => "LTC",
            "ETHEREUM" => "ETH",
            _ => throw new ArgumentException($"Invalid gateway: {gateway}")
        };
    }
}

private static async void Bot_OnMessage(object sender, MessageEventArgs e)
{
    var message = e.Message;

    if (message.Type == MessageType.Text && message.Text.StartsWith("/start"))
    {
        await Bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome! To complete your purchase, use the appropriate command."
        );
    }
    else if (message.Type == MessageType.Text && message.Text.StartsWith("/purchase"))
    {
        // Extract parameters for creating the order
        string gateway = "YOUR_GATEWAY";
        string selectedPlan = "SELECTED_PLAN";

        // Call a method to create order
        var (address, amount, uniqid, protocol, usdvalue) = CreateOrder(gateway, selectedPlan);

        // Insert order details into database
        // db_manager.InsertOrder(chat_id, user_id, username, uniqid, "PENDING", protocol, usdvalue, selectedPlan, "None");

        // Send message to user
        await Bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"To complete your purchase with {gateway}\nPlease send {amount} {currency}\nTo Address: {address}\nYour Order ID is: {uniqid}\nWe are checking for payment status, please wait for 2 Confirmations.",
            parseMode: ParseMode.Html
        );

        // Start periodic check for payment status
        // StartPeriodicCheck(chat_id, uniqid);
    }
}

// Define CreateOrder method
private static (string, decimal, string, string, decimal) CreateOrder(string gateway, string selectedPlan)
{
    // Your logic for creating an order
    // ...
    // Return order details
    return ("ADDRESS", 100, "UNIQID", "PROTOCOL", 50);
}

// Define StartPeriodicCheck method
private static void StartPeriodicCheck(long chatId, string uniqid)
{
    // Your logic for periodic payment status check
}
}