using System.Text;
using System.Text.Json;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Validators;
using CollabParty.Application.Common.Validators.Auth;
using CollabParty.Application.Common.Validators.Party;
using CollabParty.Application.Services.Implementations;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure;
using CollabParty.Infrastructure.Data;
using CollabParty.Infrastructure.DependencyInjection;
using CollabParty.Infrastructure.Emails; // Add this
using CollabParty.Infrastructure.Persistence.Seeders;
using CollabParty.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Questlog.Api.Mappings;

var builder = WebApplication.CreateBuilder(args);

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database Context Configuration
var connectionString = builder.Configuration["DefaultConnectionString"];


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// builder.Services.Configure<IdentityOptions>(options =>
// {
//     options.Tokens.PasswordResetTokenProvider = "ResetPassword";
// });
//
// builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
// {
//     // Set the expiration time of the token to 15 minutes
//     options.TokenLifespan = TimeSpan.FromMinutes(15);
// });


// builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
// {
//     options.TokenLifespan = TimeSpan.FromMinutes(15);
// });

// AutoMapper setup
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UnlockedAvatarService, UnlockedAvatarService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAvatarService, AvatarService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// Suppress Model State Validation for Custom Filters
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

// FluentValidation Setup
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Register the CamelCaseValidationInterceptor manually
builder.Services.AddScoped<IValidatorInterceptor, CamelCaseValidationInterceptor>();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<LoginCredentialsRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCredentialsRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TokenDtoRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePartyRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ForgotPasswordRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordRequestDtoValidator>();


// JSON and FluentValidation Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });

// Use the DependencyInjection method for adding EmailService
builder.Services.AddInfrastructureServices(builder.Configuration);


// JWT Authentication Configuration
var jwtKey = builder.Configuration["JwtSecret"];
var jwtIssuer = builder.Configuration["JwtIssuer"];
var jwtAudience = builder.Configuration["JwtAudience"];

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.Zero,
        };

        // Add custom logging for validation failures
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log the error or handle it in some way
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Optionally log token validation success here
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });

// Swagger / OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    AvatarSeeder.Seed(dbContext);
    await UserSeeder.Seed(dbContext, userManager);
    PartySeeder.Seed(dbContext);
    UserAvatarSeeder.Seed(dbContext);
    PartyMemberSeeder.Seed(dbContext);
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();