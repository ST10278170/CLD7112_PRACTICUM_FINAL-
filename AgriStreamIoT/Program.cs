using AgriStreamIoT.Data;
using Microsoft.EntityFrameworkCore;
using AgriStreamIoT.Services;


var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// RELATIONAL DATABASE
// Local SQL Server implementation representing Azure SQL.
// ------------------------------------------------------------

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------------------------------
// MVC
// ------------------------------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TelemetryTableService>();
builder.Services.AddScoped<DroneBlobStorageService>();
builder.Services.AddScoped<IrrigationQueueService>();

var app = builder.Build();

// ------------------------------------------------------------
// HTTP REQUEST PIPELINE
// ------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();