using BE_PRN232.Configs;
using BE_PRN232.Entities;
using BE_PRN232.Service;
using BE_PRN232.Service.ServiceImp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddScoped<IEmailService , EmailService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles(); // wwwroot
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(pathImage)),
//    RequestPath = "/images"
//});

app.UseCors("AllowClient");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();