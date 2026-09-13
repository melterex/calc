public class History : IHistory
{
    public HistoryEntry SaveToHistory(string expression, double result)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<HistoryEntry> GetAllHistory() 
    {
        throw new NotImplementedException();
    }

    public HistoryEntry? GetHistoryById(int id)
    {
        throw new NotImplementedException();
    }
}