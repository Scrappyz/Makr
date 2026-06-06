using Makr.Application.Interfaces;
using Makr.Application.Services;
using Makr.Infrastructure.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ===BIND SETTINGS===
builder.Services.Configure<TemplateSetting>(
    builder.Configuration.GetSection("Template")
);

builder.Services.AddSingleton<ITemplateSetting>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<TemplateSetting>>().Value
);

builder.Services.Configure<TemplateSetting>(
    builder.Configuration.GetSection("Template")
);

builder.Services.AddSingleton<IPostConfigureOptions<TemplateSetting>,
    TemplateSettingPostConfigure>();
// ===BIND SETTINGS===

builder.Services.AddControllers();

builder.Services.AddScoped<TemplateService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
