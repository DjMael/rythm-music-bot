using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rythm.Extensions
{
    internal class MusicList
    {
        public static DiscordUser User { get; set; }
        public static IReadOnlyList<MusicList> MusicLists {get; set;}
    }
}
