using Auth.Api.Middlewares;
using Auth.Application.Interfaces;
using Auth.Infrastructure;
using Auth.Infrastructure.Services;
using Auth.Messaging.Send.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => //dbcontext
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));
    options.UseInMemoryDatabase("InMemoryDb");
});
builder.Services.AddSwaggerGen(); //Swagger
builder.Services.Configure<RouteOptions>(options => //Lowercase url
{
    options.LowercaseUrls = true;
});
builder.Services.AddResponseCompression(options => //Response compression
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options => //Response compression
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options => //Response compression
{
    options.Level = CompressionLevel.SmallestSize;
});
builder.Services.AddAuthentication(options => //JWT
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
    };
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddTransient<IAuthService, AuthService>(); //DI
builder.Services.AddTransient<ITokenService, TokenService>(); //DI
builder.Services.AddHttpClient();
IConfigurationSection rabbitMqSection = builder.Configuration.GetSection("RabbitMq"); //RabbitMq
builder.Services.Configure<RabbitMqConfiguration>(rabbitMqSection); //RabbitMq

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>(); //Error management
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression(); //Response compression
app.UseHttpsRedirection();
app.MapControllers();
app.Run();