using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnliDesk.Server.Data;
using OnliDesk.Server.Hubs;
using OnliDesk.Server.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<OnliDeskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "OnliDesk-Super-Secret-Key-2024-Very-Long-And-Secure";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// SignalR
builder.Services.AddSignalR();

// Custom Services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ClientIdProvider>();
builder.Services.AddHostedService<CleanupService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RemoteAccessHub>("/remotehub");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    service = "OnliDesk Server"
}));

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OnliDeskDbContext>();
    context.Database.EnsureCreated();
}

Console.WriteLine("🚀 OnliDesk Server starting...");
Console.WriteLine("🌐 Server will be available at: http://localhost:5000");
Console.WriteLine("📊 Health check: http://localhost:5000/health");
Console.WriteLine("📚 API Documentation: http://localhost:5000/swagger");

app.Run();
