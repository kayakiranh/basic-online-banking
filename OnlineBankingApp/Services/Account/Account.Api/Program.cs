using Account.Api.Middlewares;
using Account.Infrastructure;
using Account.Messaging.Receive.Options;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => //dbcontext
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //Swagger
builder.Services.AddApiVersioning(); //Api Version
builder.Services.AddVersionedApiExplorer(setup =>  //Api Version
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});
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
app.UseResponseCompression(); //Response compression
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();