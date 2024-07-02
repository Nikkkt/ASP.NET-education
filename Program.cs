using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.OTP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Місце для реєстрації служб між створенням builder та його використанням (app)
// Реєстрація - співставлення інтерфейсу з класом за формулою
// "Буде запит на IHashService - видати об'єкт класу Md5HashService"

//builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IHashService, ShaHashService>();

// Homework 2
//builder.Services.AddSingleton<IOtpService, Otp6Service>();
builder.Services.AddSingleton<IOtpService, Otp4Service>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
