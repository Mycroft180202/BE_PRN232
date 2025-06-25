using BE_PRN232.Configs;
using BE_PRN232.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var pathImage = builder.Configuration["AppSettings:PathImage"];
var clientUrl = builder.Configuration["AppSettings:ClientUrl"] ?? "https://localhost:3000";
var baseUrl =  builder.Configuration["AppSettings:BaseUrl"] ?? "https://localhost:5000";
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppSettings>>().Value
);
builder.Services.AddDbContext<EcommerceClothingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnections"));
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy
            .WithOrigins(clientUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles(); // wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(pathImage)),
    RequestPath = "/images"
});

app.UseCors("AllowClient");
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();