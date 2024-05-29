using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddJsonFile($"ocelot.json", false, true);
builder.Services.AddOcelot(builder.Configuration);

WebApplication app = builder.Build();
app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseRouting();