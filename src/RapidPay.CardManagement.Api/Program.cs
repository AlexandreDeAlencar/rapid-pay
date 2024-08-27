using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RapidPay.CardManagement.Api.Endpoints;
using RapidPay.CardManagement.App;
using RapidPay.CardManagement.Domain;
using RapidPay.CardManagement.EntityFramewok;
using RapidPay.CardManagement.EntityFramework.Contexts;
using RapidPay.PaymentFees.BackgroundService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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
    .AddPaymentFeesBackgroundService()
    .AddCardManagementDomain()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    });

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

builder
    .Services
    .AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var cardManagementContext = services.GetRequiredService<CardManagementContext>();
            var userAuthContext = services.GetRequiredService<UserAuthContext>();

            bool cardManagementCreated = cardManagementContext.Database.EnsureCreated();
            bool userAuthCreated = userAuthContext.Database.EnsureCreated();

            if (cardManagementCreated)
            {
                logger.LogInformation("CardManagement database was created successfully.");
            }
            else
            {
                logger.LogInformation("CardManagement database already exists.");
            }

            if (userAuthCreated)
            {
                logger.LogInformation("UserAuth database was created successfully.");
            }
            else
            {
                logger.LogInformation("UserAuth database already exists.");
            }

            cardManagementContext.Database.Migrate();
            userAuthContext.Database.Migrate();

            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating or migrating the databases.");
            throw;
        }
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
