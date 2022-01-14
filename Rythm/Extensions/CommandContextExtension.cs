using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace Rythm.Extensions
{
    public static class CommandContextExtension
    {
        public static DiscordChannel GetChannel(this CommandContext ctx) => ctx.Member.VoiceState.Channel;

        public static async Task<LavalinkGuildConnection> GetLavalinkConnection(this CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return null;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return null;
            }

            return conn;
        }

        public static async Task<LavalinkNodeConnection> CheckConnectionStatus(this CommandContext ctx)
        {
            var channel = ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return null;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return null;
            }

            return node;
        }
    }
}
