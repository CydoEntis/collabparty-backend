using System.Diagnostics;
using System.Text;
using System.Text.Json;
// using CollabParty.Api.Middleware;
using CollabParty.Application.Common.Constants;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Validators;
using CollabParty.Application.Common.Validators.Auth;
using CollabParty.Application.Common.Validators.Helpers;
// using CollabParty.Application.Common.Validators.Helpers;
using CollabParty.Application.Common.Validators.Party;
using CollabParty.Application.Services.Implementations;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure;
using CollabParty.Infrastructure.Data;
using CollabParty.Infrastructure.DependencyInjection;
using CollabParty.Infrastructure.Emails;
using CollabParty.Infrastructure.Middleware; // Add this
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
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:5173")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Database Context Configuration
var connectionString = builder.Configuration["DefaultConnectionString"];


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, options => options.CommandTimeout(360)));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// AutoMapper setup
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ValidationHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnlockedAvatarService, UnlockedAvatarService>();
builder.Services.AddScoped<IAvatarService, AvatarService>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<IPartyMemberService, PartyMemberService>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IQuestStepService, QuestStepService>();

builder.Services.AddScoped<IQuestAssignmentService, QuestAssignmentService>();
builder.Services.AddScoped<IQuestCommentService, QuestCommentService>();

// Suppress Model State Validation for Custom Filters
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

// FluentValidation Setup
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Register the CamelCaseValidationInterceptor manually

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
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Use the DependencyInjection method for adding EmailService
builder.Services.AddInfrastructureServices(builder.Configuration);

// JWT Authentication Configuration
var jwtKey = builder.Configuration[JwtNames.JwtSecret];
var jwtIssuer = builder.Configuration[JwtNames.JwtIssuer];
var jwtAudience = builder.Configuration[JwtNames.JwtAudience];


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


        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies[CookieNames.AccessToken];

                Console.WriteLine(token);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

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
    UnlockedAvatarSeeder.Seed(dbContext);
    PartyMemberSeeder.Seed(dbContext);
    QuestSeeder.Seed(dbContext);
    QuestStepsSeeder.Seed(dbContext);
    QuestAssignmentSeeder.Seed(dbContext);
    // Commit all changes after seeding
    dbContext.SaveChanges(); // Perform only one save after all seeding is done
}


// app.UseMiddleware<CsrfMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();