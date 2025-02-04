using System.Text; // Required for encoding security keys
using Api.Models; // Importing API models (Assuming "AppUser" and other models are here)
using API.Data; // Importing Data layer (Assuming "AppDbContext" is defined here)
using Microsoft.AspNetCore.Authentication.JwtBearer; // Importing JWT Bearer Authentication
using Microsoft.AspNetCore.Identity; // Importing ASP.NET Core Identity for User Management
using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for Database Operations
using Microsoft.IdentityModel.Tokens; // Importing Token Security for JWT
using Microsoft.OpenApi.Models; // Importing OpenAPI for Swagger Documentation
using Swashbuckle.AspNetCore.SwaggerGen; // Importing Swagger Generator for API Documentation

var builder = WebApplication.CreateBuilder(args); // Creating a new instance of WebApplication builder

// ======================== Configure Services ==============================

// Configuring Database Context to use SQLite with "auth.db" as the database file
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=auth.db"));

// Configuring Identity System with AppUser as User and IdentityRole for Role Management
// Stores identity-related data in the database using Entity Framework
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); // Enables Token-based operations (e.g., Password Reset)

// Retrieving JWT settings from the configuration file (appsettings.json)
var JWTSetting = builder.Configuration.GetSection("JWTSetting");

// Configuring Authentication using JWT Bearer Tokens
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Default authentication scheme
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Default challenge scheme
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Default scheme
}).AddJwtBearer(opt => {
    opt.SaveToken = true; // Saves the token upon successful authentication
    opt.RequireHttpsMetadata = true; // Enforces HTTPS for token validation (set false for local testing)
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true, // Validates the token issuer
        ValidateAudience = true, // Validates audience (who receives the token)
        ValidateLifetime = true, // Ensures token expiration is checked
        ValidateIssuerSigningKey = true, // Validates the security key used to sign the token
        ValidAudience = JWTSetting["ValidAudience"], // Audience value from config
        ValidIssuer = JWTSetting["ValidIssuer"], // Issuer value from config
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("securityKey").Value!)) // Signing key for verification
    };
});

// Adding support for Controllers (MVC API Endpoints)
builder.Services.AddControllers();

// Adding OpenAPI (Swagger) support
builder.Services.AddEndpointsApiExplorer();

// Configuring Swagger for API documentation
builder.Services.AddSwaggerGen(c => {
    // Defining JWT Authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer eyeleijfjfbf'", // Description for Swagger UI
        Name = "Authorization", // HTTP Header name for Authorization
        In = ParameterLocation.Header, // Specifies that the token is sent in the header
        Type = SecuritySchemeType.ApiKey, // Defines it as an API key scheme
        Scheme = "Bearer" // Specifies the scheme name
    });

    // Adding security requirements for Swagger UI to authorize requests
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Matches the security definition above
                },
                Scheme = "oauth2", // Correcting typo from "outh2" to "oauth2"
                Name = "Bearer",
                In = ParameterLocation.Header, // Token must be included in the header
            },
            new List<string>() // Empty list (no scopes required for this API)
        }
    });
});

// ======================== Configure Middleware ==============================

var app = builder.Build(); // Building the application instance

// Enable Swagger UI only in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger Middleware
    app.UseSwaggerUI(); // Enable Swagger UI for API testing
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS automatically
app.UseAuthentication(); // Enables Authentication middleware
app.UseAuthorization(); // Enables Authorization middleware

app.MapControllers(); // Maps API Controllers to routes automatically

app.Run(); // Runs the application and starts listening for requests
