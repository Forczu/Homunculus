﻿using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Services;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class ReactionImagesModule : ModuleBase<SocketCommandContext>, IDiscordModule
    {
		private readonly string resources = System.AppContext.BaseDirectory + "/Resources/Images/Reactions/";

		public string ServiceName { get => "ReactionImagesModule"; }

		public ILoggingService _loggingService { get; }
		
		public ReactionImagesModule(LoggingService loggingService)
		{
			_loggingService = loggingService;
		}

		[Command("Jameson"), Alias("jjj"), Summary("JJJ laughs at your foolishness")]
		public async Task JJJAsync() => await Context.Channel.SendFileAsync(resources + "jjj.gif");
	}
}