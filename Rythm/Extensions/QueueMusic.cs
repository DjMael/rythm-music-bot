using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rythm.Extensions
{
    public static class QueueMusic
    {
        public static bool IsLoop = false;
        private static int CurrentTrack = 0;
        public static bool QueueLoop()
        {
            IsLoop = !IsLoop;
            return IsLoop;
        }
        public static List<LavalinkTrack> QueueTrack { get; set; } = new List<LavalinkTrack>();
        public static IReadOnlyList<LavalinkTrack> RealQueue { get; set; } = QueueTrack;
        public static void QueueTrackAdd(LavalinkTrack linkTrack) => QueueTrack.Add(linkTrack);
        public static void QueueTrackDellete(LavalinkTrack linkTrack) => QueueTrack.Remove(linkTrack);
        public static void QueueTrackDelleteAll() => QueueTrack.Clear();
        public static void CheckLoop()
        {
            if (IsLoop && QueueTrack.Count == (CurrentTrack + 1))
            {
                CurrentTrack = 0;
            }
        }
        public static IEnumerable<LavalinkTrack> QueueTrackSkip()
        {
            if (IsLoop && QueueTrack.Count == (CurrentTrack + 1))
            {
                QueueTrack.Skip(-CurrentTrack);
                CurrentTrack = 0;
                return QueueTrack;
            }
            else
            {
                CurrentTrack++;
                return QueueTrack.Skip(CurrentTrack);
            }

        }
        public static IEnumerable<LavalinkTrack> QueueTrackPrevious()
        {
            CurrentTrack--;
            return QueueTrack.Skip(CurrentTrack);
        }
        public static void Conn_PlaybackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            {
                if (IsLoop)
                {
                    var Music = QueueTrack.FirstOrDefault();
                    QueueTrackDellete(Music);
                    QueueTrackAdd(Music);
                }
                else{
                    var Music = QueueTrack.FirstOrDefault();
                    QueueTrackDellete(Music);
                }
            }
        }
    }
}