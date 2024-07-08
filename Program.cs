using ASP.NET_Classwork.Services.FileName;
using ASP.NET_Classwork.Services.Hash;
using ASP.NET_Classwork.Services.KDF;
using ASP.NET_Classwork.Services.OTP;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ̳��� ��� ��������� ����� �� ���������� builder �� ���� ������������� (app)
// ��������� - ������������ ���������� � ������ �� ��������
// "���� ����� �� IHashService - ������ ��'��� ����� Md5HashService"

//builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IHashService, ShaHashService>();
builder.Services.AddSingleton<IKdfService, Pbkdf1Service>();

// Homework 2
//builder.Services.AddSingleton<IOtpService, Otp6Service>();
builder.Services.AddSingleton<IOtpService, Otp4Service>();

// Homework 3
// ���� ����������� Transient, ��� ��� ������� ����� ������������ ��� �����
builder.Services.AddTransient<IFileNameService, FileNameService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
