using System.Text;
using System.Text.Json;
using CollabParty.Api.Mappings;
using CollabParty.Application.Common.Validators.Auth;
using CollabParty.Application.Common.Validators.Party;
using CollabParty.Application.Services.Implementations;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using CollabParty.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPartyService, PartyService>();

// Suppress Model State Validation for Custom Filters
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);




// JSON and FluentValidation Configuration
builder.Services.AddControllers(options =>
    {
        // Uncomment to add custom filters if needed
        // options.Filters.Add<TokenValidationFilter>();
        // options.Filters.Add<CamelCaseValidationFilter>();
    })
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// FluentValidation Setup
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<LoginCredentialsDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCredentialsDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TokenDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePartyDtoValidator>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
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

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();