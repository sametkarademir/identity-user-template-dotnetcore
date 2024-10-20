using Application;
using Core.Exceptions.Middleware;
using Domain.Shared.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Persistence;
using RabbitMQ.Client;
using WebApi.Consumers;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("MyCorsPolicy", corsPolicyBuilder =>
        //.WithOrigins("https://localhost:5173", "http://localhost:5173")
        corsPolicyBuilder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddPersistenceExtensions(builder.Configuration);
builder.Services.AddApplicationExtensions(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddControllers().AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<MailSendRequestEventHandler>();
builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "IdentityUserServiceApi",
        DefaultTopicName = "IdentityUser",
        EventBusType = EventBusType.RabbitMq,
        EventNameSuffix = "",
        Connection = new ConnectionFactory()
        {
            HostName = builder.Configuration["RabbitMq:Host"],
            Port = Convert.ToInt16(builder.Configuration["RabbitMq:Port"]),
            UserName = builder.Configuration["RabbitMq:UserName"],
            Password = builder.Configuration["RabbitMq:Password"]
        }
    };
    return EventBusFactory.Create(config, sp);
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition(
        name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer YOUR_TOKEN\". \r\n\r\n"
                + "`Enter your token in the text input below.`"
        }
    );
    opt.OperationFilter<BearerSecurityRequirementOperationFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
}
app.UseCors("MyCorsPolicy");

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region ContextSeed
using var scope = app.Services.CreateScope();
try
{
    var contextSeedService = scope.ServiceProvider.GetService<ContextSeedService>();
    await contextSeedService!.InitializeContextAsync();
}
catch(Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger!.LogError(ex.Message, "Failed to initialize and seed the database");
}
#endregion

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<MailSendRequestEvent, MailSendRequestEventHandler>();

app.Run();