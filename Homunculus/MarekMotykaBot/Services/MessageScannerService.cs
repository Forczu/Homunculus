﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class MessageScannerService
    {
        private readonly DiscordSocketClient _client;

        private List<string> _waifuList = new List<string>() { "Asuna", "Rias", "Erina" };
        private List<string> _marekFaceWords = new List<string>() { "czerń", "czarn", "nigga", "nigger", "ciemn", "murzyn", "black", "schartz", "cień", "mrok", "mroczn", "afryk", "africa", "negro", "kuro"};

        public MessageScannerService(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task ScanMessage(SocketMessage s)
        {
            var message = s as SocketUserMessage;

            if (message == null)
                return;

            var context = new SocketCommandContext(_client, message);

            if (!message.Author.IsBot)
            {
                await DetectWaifus(context, message);
                await DetectMarekFaceTriggerWords(context, message);
                await DetectMentions(context, message);
            }
        }

        private async Task DetectMentions(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.MentionedUsers.Where(x => x.DiscordId().Equals("MarekMotykaBot#2213") || x.DiscordId().Equals("Erina#5946")).FirstOrDefault() != null ||
                message.Tags.Any(x => x.Type.Equals(TagType.EveryoneMention) || x.Type.Equals(TagType.HereMention)))
            {
                await context.Channel.SendMessageAsync(StringConsts.MasterThesis);
            }
        }

        /// <summary>
        /// Check for marekface trigger words in each message and add reaction.
        /// </summary>
        private async Task DetectMarekFaceTriggerWords(SocketCommandContext context, SocketUserMessage message)
        {
            foreach (string word in _marekFaceWords)
            {
                if (message.Content.ToLowerInvariant().Contains(word) && !message.Author.IsBot)
                {
                    var marekFace = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals("marekface")).FirstOrDefault();

                    if (marekFace != null)
                        await message.AddReactionAsync(marekFace);

                    break;
                }
            }
        }

        /// <summary>
        /// Detect waidus name in each message
        /// </summary>
        private async Task DetectWaifus(SocketCommandContext context, SocketUserMessage message)
        {
            foreach (string waifuName in _waifuList)
            {
                if (message.Content.ToLowerInvariant().Contains(waifuName.ToLowerInvariant()) && !message.Author.IsBot)
                {
                    await context.Channel.SendMessageAsync(string.Format(StringConsts.MarekWaifus, waifuName));
                    break;
                }
            }
        }
    }
}