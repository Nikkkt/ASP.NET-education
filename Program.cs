using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Middleware.SessionAuth;
using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.KDF;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Місце для реєстрації служб між створенням builder та його використанням (app)
// Реєстрація - співставлення інтерфейсу з класом за формулою
// "Буде запит на IHashService - видати об'єкт класу Md5HashService"

//builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IHashService, ShaHashService>();
builder.Services.AddSingleton<IKdfService, Pbkdf1Service>();

// Homework 2
//builder.Services.AddSingleton<IOtpService, Otp6Service>();
builder.Services.AddSingleton<IOtpService, Otp4Service>();

// Homework 3
// Було використано Transient, щоб при кожному запиті генерувались різні імена
builder.Services.AddTransient<IFileNameService, FileNameService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Реєструємо контекст даних
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

// Наші Middleware
app.UseSessionAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
