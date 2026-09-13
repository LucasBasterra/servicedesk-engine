using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.Endpoints;
using ServiceDesk.Api.Services;
using ServiceDesk.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ServiceDeskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrar Servicios y Validadores de FluentValidation
builder.Services.AddSingleton<JwtService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTicketValidator>();

// 3. Configurar Esquema de Autenticación JWT Bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

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
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Configurar el Pipeline HTTP y OpenAPI/Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 5. Middlewares de Seguridad
app.UseAuthentication();
app.UseAuthorization();

// 6. Mapeo de Grupos de Endpoints
app.MapAuthEndpoints();
app.MapTicketEndpoints();
app.MapCommentEndpoints();

app.Run();