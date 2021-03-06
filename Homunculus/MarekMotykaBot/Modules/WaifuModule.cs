﻿using Discord.Commands;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class WaifuModule : ModuleBase<SocketCommandContext>, IDiscordModule
    {
        private readonly Random _rng;

        private readonly JSONSerializerService _serializer;

        private readonly List<string> _marekWaifuList;

		public string ServiceName { get => "WaifuModule"; }

		public ILoggingService _loggingService { get; }

		public WaifuModule(Random random, JSONSerializerService serializer, LoggingService loggingService)
        {
            _rng = random;
            _serializer = serializer;
			_loggingService = loggingService;

            _marekWaifuList = serializer.LoadFromFile<string>("marekWaifus.json");
        }

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, 2 waifus")]
        public async Task BetterWaifu(string firstWaifu, string secondWaifu)
        {
			var waifus = new List<string>
			{
				firstWaifu,
				secondWaifu
			};

			if (waifus.Contains("Legia"))
            {
                await Context.Channel.SendMessageAsync(StringConsts.LegiaWarszawa);
                return;
            }

            if (waifus.Contains("Asuna") && waifus.Contains("Erina"))
            {
                await Context.Channel.SendMessageAsync(StringConsts.WaifuEqual);
                return;
            }

            foreach (string waifu in _marekWaifuList)
            {
				if (waifus.Contains(waifu.RemoveEmojisAndEmotes()))
                {
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuShit, waifu));
                    return;
                }
            }

            int selectedWaifuIndex = _rng.Next(waifus.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]));

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', firstWaifu, secondWaifu));
		}

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus separated with spaces")]
        public async Task BetterWaifu(params string[] waifus)
        {
            var waifusList = waifus.ToList();
            waifusList = waifusList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            if (waifusList.Contains("Asuna") && waifusList.Contains("Erina"))
            {
                await Context.Channel.SendMessageAsync(StringConsts.WaifuEqual);
                return;
            }

            foreach (string waifu in _marekWaifuList)
            {
                if (waifusList.Contains(waifu.RemoveEmojisAndEmotes()))
                {
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuShit, waifu));
                    return;
                }
            }

            int selectedWaifuIndex = _rng.Next(waifusList.Count);

            switch (waifusList.Count)
            {
                case 0:
                case 1:
                    await Context.Channel.SendMessageAsync("...");
                    break;
                case 2:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]));
                    break;
                default:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBest, waifusList[selectedWaifuIndex]));
                    break;
			}

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', waifus));
		}

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, 2 husbandos")]
        public async Task BetterHusbando(string firstHusbando, string secondHusbando)
        {
			var husbandos = new List<string>
			{
				firstHusbando,
				secondHusbando
			};

			int selectedHusbandoIndex = _rng.Next(husbandos.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBetter, husbandos[selectedHusbandoIndex]));

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', firstHusbando, secondHusbando));
		}

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, multiple husbandos separated with spaces")]
        public async Task BetterHusbando(params string[] husbandos)
        {
            var husbandoList = husbandos.ToList();
            husbandoList = husbandoList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            int selectedHusbandoIndex = _rng.Next(husbandoList.Count);

            switch (husbandoList.Count)
            {
                case 0:
                case 1:
                    await Context.Channel.SendMessageAsync("...");
                    break;
                case 2:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBetter, husbandoList[selectedHusbandoIndex]));
                    break;
                default:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBest, husbandos[selectedHusbandoIndex]));
                    break;
			}

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', husbandos));
		}
    }
}