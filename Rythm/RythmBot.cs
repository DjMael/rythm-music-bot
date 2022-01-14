using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using System;

namespace Rythm
{
    public class RythmBot
    {
        public DiscordClient Discord { get; set; }
        public CommandsNextExtension Commands { get; set; }

        public async Task MainAsync(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable("BotKey", EnvironmentVariableTarget.User),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LoggerFactory = new LoggerFactory().AddSerilog()
            });

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "lavalink-tsh.herokuapp.com",
                Port = 80
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
            };

            var lavalink = Discord.UseLavalink();

            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "!" },
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                DmHelp = false,
                CaseSensitive = false,
            });

            Commands.RegisterCommands<RythmBotCommands>();

            await Discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);

            await Task.Delay(-1);
        }
    }
}
