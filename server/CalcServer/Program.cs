using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// TODO: remove Fake here when our classes are implemented

builder.Services.AddSingleton<ICalculator, FakeCalculator>();
builder.Services.AddScoped<IHistory, History>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=calculator.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

// 1. POST: Calculate an expression
app.MapPost("/api/calculator/calculate", ([FromBody] CalculationRequest request, ICalculator calculator, IHistory history) =>
{
    try
    {
        double result = calculator.ParseAndCompute(request.Expression);
        var historyEntry = history.SaveToHistory(request.Expression, result);

        return Results.Ok(historyEntry);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// 2. GET: Retrieve all history logs
app.MapGet("/api/calculator/history", (IHistory history) =>
{
    var allHistory = history.GetAllHistory();
    return Results.Ok(allHistory);
});

// 3. GET: Get a specific calculation by ID
app.MapGet("/api/calculator/history/{id:int}", (int id, IHistory history) =>
{
    var entry = history.GetHistoryById(id);
    return entry is not null ? Results.Ok(entry) : Results.NotFound($"History item with ID {id} not found.");
});

app.Run();


public record CalculationRequest(string Expression);
public record HistoryEntry(int Id, string Expression, double Result);

public interface ICalculator
{
    double ParseAndCompute(string expression);
}

public interface IHistory
{
    HistoryEntry SaveToHistory(string expression, double result);
    IEnumerable<HistoryEntry> GetAllHistory();
    HistoryEntry? GetHistoryById(int id);
}

// mocks for testing

public class FakeCalculator : ICalculator
{
    public double ParseAndCompute(string expression)
    {
        if (expression.Contains('+')) return 5.0;
        return 42.0;
    }
}

public class FakeHistory : IHistory
{
    private readonly List<HistoryEntry> _mockDb = new()
    {
        new HistoryEntry(1, "2 + 3", 5.0),
        new HistoryEntry(2, "10 * (4 - 2)", 20.0)
    };

    public HistoryEntry SaveToHistory(string expression, double result)
    {
        var newEntry = new HistoryEntry(_mockDb.Count + 1, expression, result);
        _mockDb.Add(newEntry);
        return newEntry;
    }

    public IEnumerable<HistoryEntry> GetAllHistory() => _mockDb;

    public HistoryEntry? GetHistoryById(int id) => _mockDb.FirstOrDefault(x => x.Id == id);
}

