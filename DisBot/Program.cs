global using  Discord;
global using  Discord.WebSocket;
global using System.Threading.Tasks;
// See https://aka.ms/new-console-template for more information
DiscordSocketClient _client;
async Task MainAsync()
{
    var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
    _client = new DiscordSocketClient(_config);
    _client.MessageReceived += CommandHendler;
    _client.Log += Log;
    var token = File.ReadAllText(@"Token.txt");
    await _client.LoginAsync(TokenType.Bot, token);
    await _client.StartAsync();
    await Task.Delay(-1);
}
Task Log(LogMessage msg)
{
    Console.WriteLine(msg.ToString());
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
            default:
                break;
        }
    }
    return Task.CompletedTask;
}
void BackProc()
{ 
    MainAsync().GetAwaiter().GetResult();
}



async Task Spawn()
{
    var builder = new ComponentBuilder()
        .WithButton("label", "custom-id");

    //await ReplyAsync("Here is a button!", components: builder.Build());
}

Thread task = new Thread(BackProc);
task.Start();
Console.WriteLine("Проверка кода");