namespace GroceryApp.Domain.Entities;

public class MasterItem
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MasterItem() { }

    public static MasterItem Create(int userId, string name) => new()
    {
        UserId = userId,
        Name = name,
        CreatedAt = DateTime.UtcNow
    };
}
