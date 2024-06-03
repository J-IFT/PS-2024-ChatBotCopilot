using copilot_chatbot.Services;
using copilot_chatbot.Utilities;
using copilot_chatbot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpClient
builder.Services.AddHttpClient<OpenAIService>();

// Ajoute le support des sessions
builder.Services.AddSession();

builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton<ExcelManager>();

// Configuration ApplicationDbContext avec SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=Database/terrabloomDatabase.db"),
    ServiceLifetime.Singleton);

var app = builder.Build();

// Configure le middleware pour utiliser les sessions
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var dbcontext = app.Services.GetRequiredService<ApplicationDbContext>();
dbcontext.Database.EnsureCreated();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
