using Line.Messaging;
using Line.Messaging.Webhooks;
using LineBot.Utils;

namespace LineBot.Handlers
{
    public class TextMessageHandler : ITextMessageHandler
    {
        private readonly ILineMessagingClient messagingClient;
        private readonly ILogger<TextMessageHandler> logger;
        public TextMessageHandler(ILogger<TextMessageHandler> logger, ILineMessagingClient messagingClient)
        {
            this.logger = logger;
            this.messagingClient = messagingClient;
        }
        public async Task<IList<ISendMessage>> MessageHandler(WebhookEventSource source, string message)
        {
            var user = await messagingClient.GetUserProfileAsync(source.UserId);
            logger.LogInformation($"{source.Type} Id: {source.Id}");

            if (message.EqualsIgnoreCase("menu"))
            {
                return new List<ISendMessage>
                {
                    // new TextMessage($"{user.DisplayName} 您好，您剛剛說的是 : {(ev.Message as TextEventMessage)?.Text}"),
                    new TemplateMessage(
                        "這是功能選單",
                        new ButtonsTemplate("請選擇以下命令",
                            actions: new List<ITemplateAction>
                            {
                                new MessageTemplateAction("現在時間", "Now"),
                                new MessageTemplateAction("貼圖", "Sticker"),
                                new PostbackTemplateAction("第二層選單", "Second Menu"),
                            })),
                };
            }
            else if (message.EqualsIgnoreCase("Rich Menu List"))
            {
                var richMenuList = await messagingClient.GetRichMenuListAsync();
                var menuNames = richMenuList.Select(m => m.Name + " " + m.RichMenuId);
                return new List<ISendMessage>
                {
                    new TextMessage($@"Rich Menu List
{string.Join(Environment.NewLine, menuNames)}"),
                };
            }
            else if (message.EqualsIgnoreCase("Now"))
            {
                return new List<ISendMessage>
                {
                    new TextMessage($"現在時間： {DateTime.Now}"),
                };
            }
            else if (message.EqualsIgnoreCase("Sticker"))
            {
                return new List<ISendMessage>
                {
                    new TextMessage("加油吧"),
                    new StickerMessage("446", "1998"),
                };
            }
            else
            {
                //回傳 hello
                return new List<ISendMessage>
                {
                    new TextMessage($"{user.DisplayName} 您好，您剛剛說的是 : {message}"),
                };
            }
        }


        public async Task<IList<ISendMessage>> PostMessageHandler(WebhookEventSource source, string text, PostbackParams @params)
        {
            if (text.EqualsIgnoreCase("Second Menu"))
            {
                return new List<ISendMessage>
                {
                    // new TextMessage($"{user.DisplayName} 您好，您剛剛說的是 : {(ev.Message as TextEventMessage)?.Text}"),
                    new TemplateMessage(
                        "這是第二層選單",
                        new ButtonsTemplate("請選擇以下命令",
                            actions: new List<ITemplateAction>
                            {
                                new MessageTemplateAction("Test1", "Test1"),
                                new MessageTemplateAction("Test2", "Test2"),
                            })),
                };
            }
            return Enumerable.Empty<ISendMessage>()
                .ToList();
        }
    }
}