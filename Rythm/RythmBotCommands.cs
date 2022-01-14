using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Rythm.Extensions;
using Rythm.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rythm
{
    public class RythmBotCommands : BaseCommandModule
    {
        [Command]
        public async Task Join(CommandContext ctx)
        {
            var node = await ctx.CheckConnectionStatus();

            if (node == null) return;

            await node.ConnectAsync(ctx.GetChannel());
            Log.Information("Conectado ao canal.");
        }

        [Command]
        public async Task Leave(CommandContext ctx)
        {
            var node = await ctx.CheckConnectionStatus();

            if (node == null) return;

            var conn = node.GetGuildConnection(ctx.GetChannel().Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {ctx.GetChannel().Name}!");
        }

        [Command]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            await Join(ctx);

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (string.IsNullOrEmpty(search))
            {
                if (conn.CurrentState.CurrentTrack == null) return;
                else
                {
                    await conn.ResumeAsync();
                    var track = conn.CurrentState.CurrentTrack;
                    await ctx.RespondAsync(Message.Resume($"Now playing {track.Title}!"));
                }
            }
            else
            {
                var loadResult = await node.Rest.GetTracksAsync(search);

                if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    await ctx.RespondAsync($"Track search failed for {search}.");
                    return;
                }

                var track = loadResult.Tracks.First();
                QueueMusic.QueueTrackAdd(track);

                if (conn.CurrentState.CurrentTrack == null)
                {
                    //conn.PlaybackFinished += QueueMusic.OnFinishTrack;

                    await conn.PlayAsync(QueueMusic.QueueTrack.FirstOrDefault());
                    await ctx.RespondAsync($"Now playing {track.Title}!");
                }
            }
        }

        [Command]
        public async Task Pause(CommandContext ctx)
        {
            var conn = await ctx.GetLavalinkConnection();

            if (conn == null) return;

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }

        [Command]
        public async Task Skip(CommandContext ctx)
        {
            var conn = await ctx.GetLavalinkConnection();

            if (conn == null) return;

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            conn.PlaybackFinished += QueueMusic.Conn_PlaybackFinished;
            var nextTrack = QueueMusic.QueueTrackSkip();
            var track = nextTrack.FirstOrDefault();
            
            try
            {
                await conn.PlayAsync(track);

            }
            catch (NullReferenceException e)
            {
                await ctx.RespondAsync("Não há mais trilhas na fila");
                return;
            }

            await ctx.RespondAsync($"Now playng {track.Title}");
        }

        [Command]
        public async Task Previous(CommandContext ctx)
        {
            var conn = await ctx.GetLavalinkConnection();
            if (conn == null) return;
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }

            var previousTrack = QueueMusic.QueueTrackPrevious();
            var track = previousTrack.FirstOrDefault();
            await conn.PlayAsync(track);
            await ctx.RespondAsync($"Now playng {track.Title}");
        }

        [Command]
        public async Task Queue(CommandContext ctx)
        {
            var queue = QueueMusic.RealQueue;
            if (queue == null || !queue.Any())
            {
                await ctx.RespondAsync("Eii, a lista ta vazia BB, toca uma pra mim!! <3");
                return;
            }
            var list = queue.Select((LavalinkTrack, index) => $"{index + 1}. {LavalinkTrack.Title}");

            var listStrig = string.Join("\n", list);
            await ctx.RespondAsync(listStrig);
        }

        [Command]
        public async Task ClearQueue(CommandContext ctx)
        {
            QueueMusic.QueueTrackDelleteAll();
            await ctx.RespondAsync("All the musics has been delleted");
        }

        [Command]
        public async Task Loop(CommandContext ctx)
        {
            
            await ctx.RespondAsync($"Now the loop is {QueueMusic.QueueLoop()}");
        }
    }
}
