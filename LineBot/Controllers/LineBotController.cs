using System.Security.Cryptography;
using System.Text;
using Line.Messaging;
using Line.Messaging.Webhooks;
using LineBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LineBot.Utils;
using LineBot.Handlers;

namespace LineBot.Controllers
{
    [ApiController]
    [Route("api/linebot")]
    public class LineBotController : ControllerBase
    {
        private readonly LineBotConfig lineBotConfig;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ILogger<LineBotController> logger;
        private readonly ILineMessagingClient messagingClient;
        private readonly LineBotApp botApp;

        public LineBotController(
            LineBotConfig config, ILineMessagingClient messagingClient,
            IHttpContextAccessor httpContextAccessor, ILogger<LineBotController> logger, LineBotApp botApp)
        {
            this.botApp = botApp;
            this.messagingClient = messagingClient;
            this.logger = logger;
            this.lineBotConfig = config;
            this.contextAccessor = httpContextAccessor;
        }

        [HttpGet("run")]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("push")]
        public async Task<IActionResult> Push(string userId, string message)
        {
            await messagingClient.PushMessageAsync(userId, message);

            return Ok();
        }

        [HttpPost("run")]
        public async Task<IActionResult> Post()
        {
            try 
            {
                var events = await this.HttpContext.Request.GetWebhookEventsAsync(lineBotConfig.ChannelSecret);
                logger.LogInformation(JsonConvert.SerializeObject(events, new Newtonsoft.Json.Converters.StringEnumConverter()));

                await botApp.RunAsync(events);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
            return Ok();

        }
    }


}