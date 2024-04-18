using copilot_chatbot.Services;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Ajoute le support des sessions
builder.Services.AddSession();

// Ajoute l'injection de IConfiguration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton<ExcelManager>();

var app = builder.Build();

// Configure le middleware pour utiliser les sessions
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
