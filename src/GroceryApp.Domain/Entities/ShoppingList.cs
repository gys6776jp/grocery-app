namespace GroceryApp.Domain.Entities;

public class ShoppingList
{
    public int Id { get; private set; }
    public string Name { get; private set; } = "買い物リスト";
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<ShoppingItem> Items { get; private set; } = new List<ShoppingItem>();

    private ShoppingList() { }

    public static ShoppingList Create(int userId, string name = "買い物リスト") => new()
    {
        UserId = userId,
        Name = name,
        CreatedAt = DateTime.UtcNow
    };
}

public class ShoppingItem
{
    public int Id { get; private set; }
    public int ListId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Quantity { get; private set; }
    public string? Memo { get; private set; }
    public bool IsChecked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ShoppingItem() { }

    public static ShoppingItem Create(int listId, string name, string? quantity, string? memo) => new()
    {
        ListId = listId,
        Name = name,
        Quantity = quantity,
        Memo = memo,
        IsChecked = false,
        CreatedAt = DateTime.UtcNow
    };

    public void ToggleCheck() => IsChecked = !IsChecked;
    public void Update(string name, string? quantity, string? memo)
    {
        Name = name; Quantity = quantity; Memo = memo;
    }
}
