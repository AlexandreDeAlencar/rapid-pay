using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RapidPay.CardManagement.Api.Endpoints;
using RapidPay.CardManagement.App;
using RapidPay.CardManagement.Domain;
using RapidPay.CardManagement.EntityFramewok;
using RapidPay.CardManagement.EntityFramework.Contexts;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services
    .AddCardManagementApplication()
    .AddEntityFrameworkConfiguration(builder.Configuration)
    .AddCardManagementDomain();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure database is created and migrations are applied
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var cardManagementContext = services.GetRequiredService<CardManagementContext>();
        var userAuthContext = services.GetRequiredService<UserAuthContext>();

        // Ensure databases are created
        cardManagementContext.Database.EnsureCreated();
        userAuthContext.Database.EnsureCreated();

        // Apply migrations automatically
        cardManagementContext.Database.Migrate();
        userAuthContext.Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapCardsEndpoints();
app.MapAuthEndpoints();

app.Run();
