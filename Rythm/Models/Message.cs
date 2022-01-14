using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rythm.Models
{
    public static class Message
    {
        const string NAME = "Rythm";
        const string ICON = "https://thereisabotforthat-storage.s3.amazonaws.com/1518671529693_rythmsquare.png";

        public static DiscordEmbed Resume(string description) => new DiscordEmbedBuilder
        {
            Title = "Resume",
            Description = description
        }.AddConfigs();

        private static DiscordEmbedBuilder AddConfigs(this DiscordEmbedBuilder discordEmbedBuilder)
        {
            discordEmbedBuilder.WithAuthor(name: NAME, iconUrl: ICON);
            return discordEmbedBuilder;
        }
    }
}
