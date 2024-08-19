using Medical.UI.Filter;
using Medical.UI.Middlewares;
using Medical.UI.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession();


builder.Services.AddScoped<AuthFilter>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICrudService, CrudService>();
builder.Services.AddScoped<ILayoutService, LayoutService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<PasswordResetMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
