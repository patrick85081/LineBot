using Line.Messaging;
using LineBot.Handlers;
using LineBot.Models;
using LineBot.Utils;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddTransient<LineMessagingClient>(p => 
    new LineMessagingClient(p.GetRequiredService<LineBotConfig>().AccessToken));
builder.Services.AddTransient<LineBotApp>();
builder.Services.AddSingleton<LineBotConfig>(p => 
        p.GetRequiredService<IConfiguration>()
            .Bind<LineBotConfig>("LineBot"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
