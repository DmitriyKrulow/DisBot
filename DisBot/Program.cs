global using  Discord;
global using  Discord.WebSocket;
global using Discord.Commands;
global using System.Threading.Tasks;
global using Newtonsoft.Json;
global using Discord.Net;
global using System.Net;
// See https://aka.ms/new-console-template for more information
//DiscordSocketClient _client;
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
    //command.Log += Log;
    //_client.Log += Log;
    var token = File.ReadAllText(@"Token.txt");
    await _client.LoginAsync(TokenType.Bot, token);
    await _client.StartAsync();

    _client.MessageUpdated += MessageUpdated;
    await Task.Delay(-1);
}
//Лог изменений в сообщениях с учетом кеширования.
async  Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }
void LoggingService(DiscordSocketClient client, CommandService command)
    {
        client.Log += Log;
        command.Log += Log;
    }
//async Task Spawn()
//{
//    ComponentBuilder builder = new ComponentBuilder()
//                .WithButton("label", "custom-id");
//    await ReplyAsync("Here is a button!", components: builder.Build());
//}

async void DownloadFile(string url, string fileName) 
{
    WebClient clientWeb;
    clientWeb = new WebClient();
    await clientWeb.DownloadFileTaskAsync(new Uri(url), fileName);
}


Task Log(LogMessage msg)
{
    //Console.WriteLine(msg.ToString());
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
        //msg.Channel.SendMessageAsync(msg.Content);
        switch (msg.Content)
        {
            case "/start":
                msg.Channel.SendMessageAsync($"Что пожелаешть? {msg.Author}");
                Console.WriteLine(msg.Content);
                break;
            case "/file":
                msg.Channel.SendMessageAsync($"Может не будем смотреть? ;) {msg.Author}");              
                break;
            default:
                if (msg.Attachments.GetType() != null)
                {
                    Console.WriteLine(msg.Attachments.ElementAt(0).ContentType);
                    Console.WriteLine(msg.Attachments.ElementAt(0).Url);
                    DownloadFile(msg.Attachments.ElementAt(0).Url, "file/" +msg.Attachments.ElementAt(0).Filename);
                }
                else 
                {
                    Console.WriteLine(msg.Content);
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