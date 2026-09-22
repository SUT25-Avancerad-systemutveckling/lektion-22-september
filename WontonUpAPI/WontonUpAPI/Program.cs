using Microsoft.EntityFrameworkCore;
using WontonUpAPI.Data;
using WontonUpAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var aiProvider = builder.Configuration["Ai:Provider"];

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

if (aiProvider == "Ollama")
{
    builder.Services.AddScoped<IAiService, OllamaService>();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";
builder.Services.AddDbContext<OrdersDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddSingleton<MenuService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    // db.Database.EnsureDeleted(); // Ta bort kommentaren om du vill radera databasen vid varje start (för utveckling)
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Wonton Up API Documentation";
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
