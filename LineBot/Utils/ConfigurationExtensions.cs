namespace LineBot.Utils
{
    public static class ConfigurationExtensions
    {
        public static T Bind<T>(this IConfiguration configuration, string key) where T : new()
        {
            var instance = new T();
            configuration.Bind(key, instance);
            return instance;
        }
    }
}