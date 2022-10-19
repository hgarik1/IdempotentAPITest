using IdempotentAPI.Cache.FusionCache.Extensions.DependencyInjection;
using IdempotentAPITest.Middleware;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisCacheUrl"]; });

// Register the FusionCache Serialization (e.g. NewtonsoftJson).
// This is needed for the a 2nd-level cache.
builder.Services.AddFusionCacheNewtonsoftJsonSerializer();

// Register the IdempotentAPI.Cache.FusionCache.
// Optionally: Configure the FusionCacheEntryOptions.
builder.Services.AddIdempotentAPIUsingFusionCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CustomExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
