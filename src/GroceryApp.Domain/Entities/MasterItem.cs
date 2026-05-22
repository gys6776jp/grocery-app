namespace GroceryApp.Domain.Entities;

public class MasterItem
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Memo { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MasterItem() { }

    public static MasterItem Create(int userId, string name, string? memo = null) => new()
    {
        UserId = userId,
        Name = name,
        Memo = memo,
        CreatedAt = DateTime.UtcNow
    };

    public void Update(string name, string? memo)
    {
        Name = name;
        Memo = memo;
    }
}
