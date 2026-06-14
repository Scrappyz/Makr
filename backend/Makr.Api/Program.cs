using Makr.Application.Interfaces;
using Makr.Application.Pipeline.Interpolator;
using Makr.Application.Pipeline.PathSelector;
using Makr.Application.Pipeline.RuleEvaluator;
using Makr.Application.Services.Template;
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

// ===ADD SERVICES===
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IPathSelector, PathSelector>();
builder.Services.AddScoped<IInterpolator, Interpolator>();
builder.Services.AddScoped<IRuleEvaluator, RuleEvaluator>();
// ===ADD SERVICES===

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
