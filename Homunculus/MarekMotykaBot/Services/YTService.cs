﻿using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace MarekMotykaBot.Services
{
    public class YTService
    {
        private readonly YouTubeService _youTubeService;
        private readonly IConfiguration _configuration;

        public YTService(IConfiguration configuration)
        {
            _configuration = configuration;

            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _configuration["tokens:youtube"],
                MaxUrlLength = 256
            });
        }

        public async Task<Youtube.SearchResult> SearchYoutubeAsync(string query, string dataType)
        {
            var request = _youTubeService.Search.List("snippet");
            request.Q = query;
            request.MaxResults = 10;

            var result = await request.ExecuteAsync();
            return result.Items.FirstOrDefault(x => x.Id.Kind == dataType);
        }
    }
}