namespace GroceryApp.Domain.Entities;

public class ShoppingHistory
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public ICollection<ShoppingHistoryItem> Items { get; private set; } = new List<ShoppingHistoryItem>();

    private ShoppingHistory() { }

    public static ShoppingHistory Create(int userId) => new()
    {
        UserId = userId,
        CompletedAt = DateTime.UtcNow
    };
}

public class ShoppingHistoryItem
{
    public int Id { get; private set; }
    public int HistoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Quantity { get; private set; }
    public string? Memo { get; private set; }
    public bool IsChecked { get; private set; }

    private ShoppingHistoryItem() { }

    public static ShoppingHistoryItem Create(int historyId, string name, string? quantity, string? memo, bool isChecked) => new()
    {
        HistoryId = historyId,
        Name = name,
        Quantity = quantity,
        Memo = memo,
        IsChecked = isChecked
    };
}
