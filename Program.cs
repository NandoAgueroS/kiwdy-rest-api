using KiwdyAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.WebHost.ConfigureKestrel(serverOptions => serverOptions.ListenAnyIP(5199));
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["TokenAuthentication:Issuer"],
            ValidAudience = configuration["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"])
            ),
        };
    });

var connection = configuration["ConnectionStrings:MySql"];
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
