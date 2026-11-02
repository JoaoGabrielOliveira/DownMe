using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Service.Data;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DownloadRepository>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DownloadContext")
        ?? throw new InvalidOperationException("Connection string" + "'DefaultConnection' not found.");
        
    var stringBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
    stringBuilder.Mode = Microsoft.Data.Sqlite.SqliteOpenMode.ReadWriteCreate;
    stringBuilder.ForeignKeys = true;
    
    

    if (!File.Exists(stringBuilder.DataSource))
    {
        throw new InvalidOperationException("Database file was not fould");
    }

    options.UseSqlite(stringBuilder.ToString());
});

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped(typeof (IMessageService<>), typeof(RabbitMessageService<>));
builder.Services.AddScoped<IDownloadService, DownloadService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();