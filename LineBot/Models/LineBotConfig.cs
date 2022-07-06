namespace LineBot.Models
{
    public class LineBotConfig
    {
        [ConfigurationKeyName("channelSecret")]
        public string ChannelSecret { get; set; }

        [ConfigurationKeyName("accessToken")]
        public string AccessToken { get; set; }
    }
}