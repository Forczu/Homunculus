﻿using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();

        }

        public async Task InstallCommands()
        {

            _client.MessageReceived += HandleCommandAsync;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var message = s as SocketUserMessage;

            if (message == null)
                return;

            var context = new SocketCommandContext(_client, message);

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);

                if (result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}