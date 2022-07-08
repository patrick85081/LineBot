using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Linq;

namespace LineBot.Handlers
{
    public class LineBotApp : WebhookApplication
    {
        ILogger<LineBotApp> logger;
        ILineMessagingClient messagingClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITextMessageHandler textMessageHandler;

        public LineBotApp(
            ILogger<LineBotApp> logger, 
            ILineMessagingClient lineMessagingClient, 
            IHttpContextAccessor httpContextAccessor, 
            ITextMessageHandler textMessageHandler)
        {
            this.textMessageHandler = textMessageHandler;
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

        protected override async Task OnPostbackAsync(PostbackEvent ev)
        {
            IList<ISendMessage>? result = null;
            logger.LogInformation(httpContextAccessor.HttpContext!.Request.GetDisplayUrl());

            result = await textMessageHandler.PostMessageHandler(ev.Source, ev.Postback.Data, ev.Postback.Params);
            if (result != null)
                await messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
        }

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            logger.LogInformation(httpContextAccessor.HttpContext!.Request.GetDisplayUrl());
            IList<ISendMessage>? result = null;

            logger.LogInformation(ev.Message.GetType().FullName);

            switch (ev.Message)
            {
                case TextEventMessage message:
                    result = await textMessageHandler.MessageHandler(ev.Source, message.Text);
                    break;

                case FileEventMessage file:
                    await messagingClient.ReplyMessageAsync("很高興聽到你的檔案");
                    break;

                case MediaEventMessage media:
                    var content = (media.Type) switch
                    {
                        EventMessageType.Image => "圖片",
                        EventMessageType.Audio => "聲音",
                        EventMessageType.File => "檔案",
                        _ => "檔案",
                    };
                    var stream = await messagingClient.GetContentBytesAsync(media.Id);
                    await File.WriteAllBytesAsync(media.Id + ".jpg", stream);
                    result = new List<ISendMessage>()
                    {
                        new TextMessage($"很高興收到你的 {content}"),
                    };
                    break;

                case LocationEventMessage location:
                    break;

                case StickerEventMessage sticker:
                    result = new List<ISendMessage>
                    {
                        new StickerMessage("446", "1998", 
                            new QuickReply(new List<QuickReplyButtonObject>()
                            {
                                new QuickReplyButtonObject(new MessageTemplateAction("讚噢", "Good")),
                                new QuickReplyButtonObject(new MessageTemplateAction("現在時間", "Now"))
                            })),
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
                var returnUri = new UriBuilder(authority + uri) { Scheme = Uri.UriSchemeHttps, Port = 443 }.ToString();
                logger.LogInformation(returnUri);
                return returnUri;
            }

            return new UriBuilder(uri) { Scheme = Uri.UriSchemeHttps, Port = 443 }.ToString();
        }
    }
}