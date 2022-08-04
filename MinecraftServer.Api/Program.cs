
using MinecraftServer.Api;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Routes;
using MinecraftServer.Api.Middlewares;
using MinecraftServer.Api.MongoModels;

/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        config.GetConnectionString("Redis");
});

MongoDBServiceDI.RegistrarDI(builder.Services, config);

builder.Services.AddSingleton<IRedisService, RedisService>();

var app = builder.Build();

ModPackRoute.CriarRota(app);
LauncherVersionRoute.CriarRota(app);

if (app.Environment.IsDevelopment())
{
    app.CriarMiddlewareCasimiro();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();

public static class MongoDBServiceDI {

    public static void RegistrarDI(this IServiceCollection services, IConfigurationRoot config)
    {
        services.Configure<MongoDatabaseSettings>(options => config.GetSection("MongoConnections").Bind(options));
        services.AddScoped<ModPackMongoDBService>();
        services.AddScoped<LauncherVersionMongoDBService>();

    }
}