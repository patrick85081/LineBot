using Line.Messaging;
using Line.Messaging.Webhooks;

namespace LineBot.Handlers
{
    public interface ITextMessageHandler
    {
        Task<IList<ISendMessage>> MessageHandler(WebhookEventSource source, string text);
        Task<IList<ISendMessage>> PostMessageHandler(WebhookEventSource source, string text, PostbackParams @params);
    }
}