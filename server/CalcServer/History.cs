using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<HistoryEntry> Calculations { get; set; }
}

public class History : IHistory
{
    private readonly AppDbContext _db;

    public History(AppDbContext db) => _db = db;
    
    public HistoryEntry SaveToHistory(string expression, double result)
    {
        var entry = new HistoryEntry(0, expression, result);

        _db.Calculations.Add(entry);
        _db.SaveChanges();

        return entry;
    }

    public IEnumerable<HistoryEntry> GetAllHistory() => _db.Calculations.ToList();
    public HistoryEntry? GetHistoryById(int id) => _db.Calculations.Find(id);
}
