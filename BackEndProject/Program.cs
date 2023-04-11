

using BackEndProject.DAL;
using BackEndProject.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<LayoutService>();
// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ProductDbContext>(opt =>
{
	opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//builder.Services.AddIdentity<User, IdentityRole>(opt =>
//{
//    opt.Password.RequiredLength = 8;
//    opt.Password.RequiredUniqueChars = 2;
//    opt.Password.RequireNonAlphanumeric = false;

//    opt.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnm123456789_-";

//    opt.Lockout.MaxFailedAccessAttempts = 5;
//    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//}).AddEntityFrameworkStores<ProniaDbContext>();

builder.Services.AddHttpContextAccessor();
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
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
	);
	endpoints.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
