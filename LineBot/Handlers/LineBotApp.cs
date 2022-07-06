using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace LineBot.Handlers
{
    public class LineBotApp : WebhookApplication
    {
        ILogger<LineBotApp> logger;
        LineMessagingClient messagingClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LineBotApp(ILogger<LineBotApp> logger, LineMessagingClient lineMessagingClient, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.messagingClient = lineMessagingClient;
        }

        protected override async Task OnUnfollowAsync(UnfollowEvent ev)
        {
            logger.LogInformation($"Unfollow {JsonConvert.SerializeObject(ev)}");
        }

        protected override async Task OnFollowAsync(FollowEvent ev)
        {
            logger.LogInformation($"Follow {JsonConvert.SerializeObject(ev)}");
        }

        protected override async Task OnAccountLinkAsync(AccountLinkEvent ev)
        {
            logger.LogInformation($"Account Line {JsonConvert.SerializeObject(ev)}");
        }

        public HashSet<string> channelIdList {get;}= new HashSet<string>();
        public HashSet<string> userIdList {get;}= new HashSet<string>();

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            logger.LogInformation(httpContextAccessor.HttpContext!.Request.GetDisplayUrl());
            List<ISendMessage>? result = null;

            switch (ev.Message)
            {
                case TextEventMessage message:
                    //頻道Id
                    var channelId = ev.Source.Id;
                    //使用者Id
                    var userId = ev.Source.UserId;

                    var user = await messagingClient.GetUserProfileAsync(userId);
                    logger.LogInformation($"{ev.Source.Type} Id: {ev.Source.Id}");

                    channelIdList.Add(channelId);
                    userIdList.Add(userId);

                    //回傳 hello
                    result = new List<ISendMessage>
                    {
                        new TextMessage($"{user.DisplayName} 您好，您剛剛說的是 : {(ev.Message as TextEventMessage)?.Text}"),
                        // new VideoMessage(GetUri(httpContextAccessor, "/video.mp4"), GetUri(httpContextAccessor, "/image.png")),
                        // new ImageMessage(GetUri(httpContextAccessor, "/image.png"), GetUri(httpContextAccessor, "/image.png")),
                    };
                    break;
            }

            if (result != null)
                await messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
        }


        private string GetUri(IHttpContextAccessor httpContextAccessor, string uri)
        {
            var requestUrl = httpContextAccessor.HttpContext!.Request.GetEncodedUrl();

            var authority = new Uri(requestUrl).GetLeftPart(UriPartial.Authority);

            if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                var returnUri = new UriBuilder(authority + uri){Scheme = Uri.UriSchemeHttps, Port = 443}.ToString();
                logger.LogInformation(returnUri);
                return returnUri;
            }

            return new UriBuilder(uri){Scheme = Uri.UriSchemeHttps, Port = 443}.ToString();
        }
    }
}