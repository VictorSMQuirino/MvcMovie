using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using MvcMovie.Data.Security;
using MvcMovie.Data.Security.Interface;
using MvcMovie.Middlewares;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUtils,Utils>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MvcMovieContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext")));
}
else
{
    //TODO: configurar MySql
}

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseMiddleware<JwtTokenMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
