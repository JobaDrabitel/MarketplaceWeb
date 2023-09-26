using Marketplace_Web.Pages;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication
builder.Services.AddSession();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Use authentication
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
