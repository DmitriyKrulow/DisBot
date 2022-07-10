global using Discord;
global using Discord.Commands;
global using Discord.WebSocket;
global using System.IO;
global using System.Net;
global using System.Threading.Tasks;
bool saveFlag = false;
async Task MainAsync()
{
    var _config = new DiscordSocketConfig
    {
        MessageCacheSize = 100
    };
    var command = new CommandService();
    DiscordSocketClient _client = new DiscordSocketClient(_config);
    _client.MessageReceived += CommandHendler;
    LoggingService(_client, command);
    var token = File.ReadAllText(@"Token.txt");
    await _client.LoginAsync(TokenType.Bot, token);
    await _client.StartAsync();
    _client.MessageUpdated += MessageUpdated;
    await Task.Delay(-1);
}
async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
{
    var message = await before.GetOrDownloadAsync();
    Console.WriteLine($"{message} -> {after}");
}
void LoggingService(DiscordSocketClient client, CommandService command)
{
    client.Log += Log;
    command.Log += Log;
}
async void DownloadFile(string url, string fileName)
{
    WebClient clientWeb;
    clientWeb = new WebClient();
    await clientWeb.DownloadFileTaskAsync(new Uri(url), fileName);
}
static void PrintFile(string targetDirectory, SocketMessage msg)
{
    string[] fileEntries = Directory.GetFiles(targetDirectory);
    foreach (string fileName in fileEntries)
        ProcessFile(fileName, msg);
}
static void ProcessFile(string path, SocketMessage msg)
{
    Console.WriteLine("Processed file '{0}'.", path);
    msg.Channel.SendFileAsync(path);
}
Task Log(LogMessage msg)
{
    if (msg.Exception is CommandException cmdException)
    {
        Console.WriteLine($"[Command/{msg.Severity}] {cmdException.Command.Aliases.First()}"
            + $" failed to execute in {cmdException.Context.Channel}.");
        Console.WriteLine(cmdException);
    }
    else
        Console.WriteLine($"[General/{msg.Severity}] {msg}");
    return Task.CompletedTask;
}
Task CommandHendler(SocketMessage msg)
{

    if (!msg.Author.IsBot)
    {
        switch (msg.Content)
        {
            case "/start":
                if (saveFlag)
                    msg.Channel.SendMessageAsync($"Поработал, обращайся ещё {msg.Author}");
                else
                    msg.Channel.SendMessageAsync($"Начинаю сохранять файлы {msg.Author}");
                saveFlag = !saveFlag;
                Console.WriteLine(msg.Content);
                Console.WriteLine(saveFlag);
                break;
            case "/file":
                msg.Channel.SendMessageAsync($"Может не будем смотреть? ;) {msg.Author}");
                PrintFile(@"file", msg);
                break;
            default:
                if ((msg.Attachments.GetType() != null) && saveFlag)
                {
                    Console.WriteLine(msg.Attachments.ElementAt(0).ContentType);
                    Console.WriteLine(msg.Attachments.ElementAt(0).Url);
                    DownloadFile(msg.Attachments.ElementAt(0).Url, "file/" + msg.Attachments.ElementAt(0).Filename);
                }
                break;
        }
    }
    return Task.CompletedTask;
}
void BackProc()
{
    MainAsync().GetAwaiter().GetResult();
}
Thread task = new Thread(BackProc);
task.Start();
Console.WriteLine("Проверка кода");