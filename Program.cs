using IsLabApp.Models;
using IsLabApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<INoteService, InMemoryNoteService>();

var app = builder.Build();

// Стандартный WeatherForecast
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Диагностические эндпоинты
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow });
})
.WithName("HealthCheck");

app.MapGet("/version", (IConfiguration config) =>
{
    var appName = config["App:Name"] ?? "IsLabApp";
    var appVersion = config["App:Version"] ?? "0.0.1";
    return Results.Ok(new { name = appName, version = appVersion });
})
.WithName("GetVersion");

// API Заметок
app.MapGet("/api/notes", (INoteService noteService) =>
{
    return Results.Ok(noteService.GetAll());
})
.WithName("GetAllNotes");

app.MapGet("/api/notes/{id}", (int id, INoteService noteService) =>
{
    var note = noteService.GetById(id);
    return note is null ? Results.NotFound() : Results.Ok(note);
})
.WithName("GetNoteById");

app.MapPost("/api/notes", (Note note, INoteService noteService) =>
{
    if (string.IsNullOrWhiteSpace(note.Title) || string.IsNullOrWhiteSpace(note.Text))
    {
        return Results.BadRequest("Title and Text are required.");
    }
    var created = noteService.Create(note);
    return Results.Created($"/api/notes/{created.Id}", created);
})
.WithName("CreateNote");

app.MapDelete("/api/notes/{id}", (int id, INoteService noteService) =>
{
    var deleted = noteService.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteNote");

// Заглушка проверки БД
app.MapGet("/db/ping", (IConfiguration config) =>
{
    var connectionString = config.GetConnectionString("Mssql");
    if (string.IsNullOrEmpty(connectionString))
    {
        return Results.Problem("Connection string 'Mssql' not configured.");
    }
    return Results.Ok(new { status = "not implemented", message = "SQL Server connection will be checked later." });
})
.WithName("DbPing");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}