using System.Security.Claims;
using System.Text;
using Fracto.Backend.Services;
using Fracto.Backend.Services.Interface;
using Fracto.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------------- CORS ----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200") // your Angular dev URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Adding services
builder.Services.AddControllers();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Add this line so the Role claim works correctly
        RoleClaimType = ClaimTypes.Role
    };
});


// Add services to the container.
builder.Services.AddDbContext<FractoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();//Testing done via Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAngularDev");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

