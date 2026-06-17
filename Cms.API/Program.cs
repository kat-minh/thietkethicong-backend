using System.Text;
using Cms.API.Middleware;
using Cms.Repository.Extensions;
using Cms.Repository.Persistence;
using Cms.Service.Extensions;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Layered services
// ---------------------------------------------------------------------------
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddApplicationServices(builder.Configuration);

// ---------------------------------------------------------------------------
// CORS — allow the Next.js zones (client + admin) to call the API
// ---------------------------------------------------------------------------
const string CorsPolicy = "NextjsFrontend";

// Origins can be overridden via config "Cors:AllowedOrigins" (array) or the
// env var Cors__AllowedOrigins__0, Cors__AllowedOrigins__1, ...
var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[]
    {
        "http://localhost:3000", // client zone (and admin via its /admin proxy)
        "http://localhost:3001", // admin zone hit directly in dev
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ---------------------------------------------------------------------------
// JWT Authentication
// ---------------------------------------------------------------------------
var jwt = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// MVC + Swagger (with Bearer support)
// ---------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // used by RevalidationMiddleware to ping the site
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BMT CMS API", Version = "v1" });

    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token (without the 'Bearer ' prefix).",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Apply migrations + seed on startup (with retry while Postgres warms up)
// ---------------------------------------------------------------------------
await InitializeDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RevalidationMiddleware>(); // ping site to revalidate after content mutations
app.MapControllers();

app.Run();

// ---------------------------------------------------------------------------
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    const int maxRetries = 10;
    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Initializing database (attempt {Attempt}/{Max})...", attempt, maxRetries);

            var db = services.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();

            var seeder = services.GetRequiredService<IDatabaseSeeder>();
            await seeder.SeedAsync();

            logger.LogInformation("Database is ready.");
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database not ready yet. Retrying in 3 seconds...");
            await Task.Delay(3000);
        }
    }

    throw new InvalidOperationException("Could not connect to the database after multiple attempts.");
}
