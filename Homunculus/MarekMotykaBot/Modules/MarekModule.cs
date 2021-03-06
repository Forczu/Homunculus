﻿using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Caches;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class MarekModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly ImgurService _imgur;
		private readonly JSONSerializerService _serializer;
		private readonly ImgFlipService _imgFlip;
		private readonly Random _rng;
		private readonly IConfiguration _configuration;

		private readonly List<string> eightBallResponses;

		private const byte cacheSize = 10;
		
		public string ServiceName { get => "MarekModule"; }

		public ILoggingService _loggingService { get; }

		public MarekModule(IConfiguration configuration, ImgurService imgur, JSONSerializerService serializer, ImgFlipService imgFlip, Random random, LoggingService loggingService)
		{
			_configuration = configuration;
			_imgur = imgur;
			_serializer = serializer;
			_rng = random;
			_imgFlip = imgFlip;
			_loggingService = loggingService;

			eightBallResponses = _serializer.LoadFromFile<string>("8ballResponses.json");
		}

		[Command("NoCoSeMoge"), Alias("no"), Summary("He will tell you what you can do")]
		public async Task CoSeMogeAsync()
		{
			await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
			await Task.Delay(3000);

			int hitNumber = _rng.Next(0, 15);

			if (hitNumber == 1)
			{
				await Context.Channel.SendMessageAsync($"...");
				await Task.Delay(1000);
				await Context.Channel.SendMessageAsync($"...");
				await Task.Delay(1000);
				await Context.Channel.SendMessageAsync($"**{StringConsts.RunAway}**");
				return;
			}

			int damageNumber = _rng.Next(0, 15);
			switch (damageNumber)
			{
				case (1):
					await Context.Channel.SendMessageAsync($"**{StringConsts.EggString}**");
					break;

				case (2):
				case (3):
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					await Task.Delay(1500);
					await Context.Channel.SendMessageAsync($"**{StringConsts.InTheAss}**");
					break;

				case (4):
					await Context.Channel.SendMessageAsync($"**{StringConsts.FinishingBlow}**");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"https://i.imgur.com/f0F9eqK.png");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"**{StringConsts.GenkiDama}**");
					break;

				case (5):
				case (6):
				case (7):
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					await Task.Delay(1500);
					await Context.Channel.SendMessageAsync($"**{StringConsts.InTheJar}**");
					break;

				default:
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					break;
			}

			switch (hitNumber)
			{
				case (2):
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync(StringConsts.MissedThrow2);
					break;

				case (3):
				case (4):
					string victim = Context.Guild.GetRandomUserName(_rng, Context.User.DiscordId());
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync(string.Format(StringConsts.MissedThrow, victim));
					break;

				default:
					break;
			}


			List<DeclineCache> declineCache = _serializer.LoadFromFile<DeclineCache>("declineCache.json");

			declineCache.RemoveAll(x => x.DiscordUsername.Equals(Context.User.DiscordId()));

			_serializer.SaveToFile("declineCache.json", declineCache);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("Sowa"), Alias("owl"), Summary("Post random owl image")]
		public async Task SowaAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromGallery("CbtU3");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Post random old Marek meme image")]
		public async Task OldMemeAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("V5CPd");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
        }

        [Command("LonkMeme"), Alias("lonk"), Summary("Post random Lonk meme image")]
        public async Task LonkMemeAsync()
        {
            string picUrl = await _imgur.GetRandomImageFromAlbum("w5dzWtL");

            await ReplyAsync(picUrl);

            _loggingService.CustomCommandLog(Context.Message, ServiceName);
        }

        [Command("MarekMeme"), Alias("meme"), Summary("Create your own Marek meme image, text split by semicolon - marekface version")]
		public async Task NewMemeAsync(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			if (captions.Count < 2)
				return;

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return;

			string toptext = captions[0].ToUpper();
			string bottomtext = captions[1].ToUpper();

			string resultUrl = await _imgFlip.CreateMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', captions));
		}

		[Command("DrakeMeme"), Alias("drake"), Summary("Create your own Marek meme image, text split by semicolon - Marek Drake version")]
		public async Task DrakeMemeAsync(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			if (captions.Count < 2)
				return;

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return;

			string toptext = captions[0].ToUpper();
			string bottomtext = captions[1].ToUpper();

			string resultUrl = await _imgFlip.CreateDrakeMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', captions));
		}

		[Command("MarekMeme2"), Alias("meme2"), Summary("Create your own Marek meme image, text split by semicolon - laughing version")]
		public async Task NewMeme2Async(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			if (captions.Count < 2)
				return;

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return;

			string toptext = captions[0].ToUpper();
			string bottomtext = captions[1].ToUpper();

			string resultUrl = await _imgFlip.CreateLaughingMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', captions));
		}

		[Command("Joke"), Summary("Marek's joke - you know the drill")]
		public async Task JokeAsync()
		{
			List<OneLinerJoke> jokes = _serializer.LoadFromFile<OneLinerJoke>("oneLiners.json");

			int randomJokeIndex = _rng.Next(1, jokes.Count);

			OneLinerJoke selectedJoke = jokes[randomJokeIndex];

			await Context.Channel.SendMessageAsync($"{selectedJoke.Question}");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"{selectedJoke.Punchline}");

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("8ball"), Summary("Binary answer for all your questions")]
		public async Task EightBallAsync(params string[] text)
		{
			string messageKey = string.Join(" ", text);
			string userKey = Context.User.DiscordId();

			if (string.IsNullOrWhiteSpace(messageKey))
			{
				await Context.Channel.SendMessageAsync(StringConsts.WrongQuestion);
			}
			else
			{
				List<EightBallCache> cache = _serializer.LoadFromFile<EightBallCache>("cache8ball.json");
				
				// Check if message was not received earlier; If yes, send same answer
				if (cache.Exists(x => x.Question == messageKey && x.DiscordUsername == userKey))
				{
					await Context.Channel.SendMessageAsync(cache.Find(x => x.Question == messageKey).Answer);
				}
				else
				{
					int randomResponseIndex = _rng.Next(1, eightBallResponses.ToList().Count);

					string selectedResponse = eightBallResponses.ElementAt(randomResponseIndex);

					string selectedUser = Context.Guild.GetRandomUserName(_rng);

					if (cache.Count > cacheSize)
						cache.RemoveAt(0);

					cache.Add(new EightBallCache(userKey, messageKey, string.Format(selectedResponse, selectedUser)));

					_serializer.SaveToFile<EightBallCache>("cache8ball.json", cache);

					await Context.Channel.SendMessageAsync($"{string.Format(selectedResponse, selectedUser)}");
				}
			}

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', messageKey));
		}

		[Command("quote"), Alias("cytat", "q"), Summary("Ancient wisdom...")]
		public async Task QuoteAsync(params string[] category)
		{
			QuoteCategory filtercategory = QuoteCategory.None;

			if (category.Length != 0)
			{
				switch (category[0].ToLower())
				{
					case ("i"):
					case ("p"):
					case ("insult"):
					case ("pocisk"):
						filtercategory = QuoteCategory.Insult;
						break;
					case ("m"):
					case ("w"):
					case ("mądrość"):
					case ("wisdom"):
						filtercategory = QuoteCategory.Wisdom;
						break;
					case ("t"):
					case ("thought"):
						filtercategory = QuoteCategory.Thought;
						break;
					case ("f"):
					case ("fiutt"):
						filtercategory = QuoteCategory.Fiutt;
						break;
					case ("reaction"):
					case ("reakcja"):
					case ("r"):
						filtercategory = QuoteCategory.Reaction;
						break;
					case ("d"):
						filtercategory = QuoteCategory.OfTheDay;
						break;
					default:
						break;
				}
			}

			Quote selectedQuote;

			switch (filtercategory)
			{
				case QuoteCategory.OfTheDay:
					selectedQuote = _serializer.LoadFromFile<Quote>("quoteOfTheDay.json").First();
					break;
				case QuoteCategory.Insult:
					var remorseChance = _rng.Next(0, 20);
					if (remorseChance == 3)
					{
						await Context.Channel.SendMessageAsync("...");
						await Task.Delay(3000);
						await Context.Channel.SendMessageAsync(StringConsts.WhyWouldIDoThat);
						return;
					}
					selectedQuote = GetRandomQuote(filtercategory);
					break;
				default:
					selectedQuote = GetRandomQuote(filtercategory);
					break;
			}

			var builder = new EmbedBuilder();

			builder.WithFooter(selectedQuote.Author);
			builder.WithTitle(selectedQuote.QuoteBody);

			string intro = string.Empty;

			switch (_rng.Next(1, 4))
			{
				case 1:
					intro = StringConsts.DerpQuote;
					break;

				case 2:
					intro = StringConsts.DerpQuote2;
					break;

				case 3:
					intro = StringConsts.DerpQuote3;
					break;
			}

            int lateArrivalProbability = _rng.Next(0, 10);
            if (lateArrivalProbability == 1)
            {
                await Task.Delay(5000);
                int lateMessageId = _rng.Next(0, 2);
                string lateArrivalMessage = lateMessageId == 0 
                    ? StringConsts.SorryForLateArrivalMessage1 
                    : StringConsts.SorryForLateArrivalMessage2;
                await Context.Channel.SendMessageAsync(lateArrivalMessage);
                await Task.Delay(3000);
            }

			await Context.Channel.SendMessageAsync(intro);
			await Task.Delay(3000);
			await ReplyAsync("", false, builder.Build());

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', category));
		}

		[Command("blueribbon"), Summary("Passes for hidden gift")]
		public async Task UnityAsync()
		{
			if (!Context.User.DiscordId().Equals("Tarlfgar#9358"))
			{
				await Context.Channel.SendMessageAsync(String.Format(StringConsts.SecretGiftDeny, "Lonka!"));
			}
			else
			{
				await Context.Channel.SendMessageAsync("Helion user e-mail: " + _configuration["credentials:helionUser"]);
				await Context.Channel.SendMessageAsync("Helion password: " + _configuration["credentials:helionPassword"]);
			}

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("lastContact"), Alias("lc", "lastMessage", "lm"), Summary("Last message by Marek")]
		public async Task LastContactAsync()
		{
			LastMarekMessage lastMessage = _serializer.LoadSingleFromFile<LastMarekMessage>("marekLastMessage.json");

			if (lastMessage != null)
			{
				var builder = new EmbedBuilder();

				string footerSuffix = string.Empty;

				int daysDifference = (DateTime.Now.Date - lastMessage.DatePosted.Date).Days;

				switch (daysDifference)
				{
					case (0):
						footerSuffix = StringConsts.Today;
						break;
					case (1):
						footerSuffix = StringConsts.Yesterday;
						break;
					default:
						footerSuffix = string.Format(StringConsts.DaysAgo, daysDifference);
						break;
				}

				builder.WithFooter(lastMessage.DatePosted.ToString("yyyy-MM-dd HH:mm") + ", " + footerSuffix);

				if (lastMessage.IsImage)
					builder.WithImageUrl(lastMessage.MessageContent);
				else
					builder.WithTitle(lastMessage.MessageContent.Truncate(250));

				await ReplyAsync("", false, builder.Build());
			}
		}

        [Command("suchar"), Alias("pun"), Summary("A derpish pun from a derpish member")]
        public async Task DerpPunAsync()
        {
            var suchars = _serializer.LoadFromFile<OneLinerJoke>("derpSuchars.json");
            var selectedSucharIndex = _rng.Next(0, suchars.Count);
            var selectedSuchar = suchars[selectedSucharIndex];

            await Context.Channel.SendMessageAsync($"{selectedSuchar.Question}");
            await Task.Delay(3000);
            await Context.Channel.SendMessageAsync($"{selectedSuchar.Punchline}");

            _loggingService.CustomCommandLog(Context.Message, ServiceName);
        }

		private Quote GetRandomQuote(QuoteCategory filtercategory)
		{
			// secret quote...
			var secretQuoteResult = _rng.Next(1, 50);
			if (secretQuoteResult == 2)
			{
				var secretQuote = string.Format(StringConsts.DeclineCommand, "!q");
				var author = "Sztuczny Murzyn";
				return new Quote(secretQuote, author, null);
			}
			else
			{
				List<Quote> quotes = _serializer.LoadFromFile<Quote>("quotes.json");

				if (filtercategory != QuoteCategory.None)
					quotes = quotes.Where(x => x.Categories.Contains(filtercategory)).ToList();

				int randomQuoteIndex = _rng.Next(0, quotes.Count);

				return quotes[randomQuoteIndex];
			}
		}
    }
}