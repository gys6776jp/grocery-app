namespace GroceryApp.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public ICollection<ShoppingList> ShoppingLists { get; private set; } = new List<ShoppingList>();
    public ICollection<MasterItem> MasterItems { get; private set; } = new List<MasterItem>();

    private User() { }

    public static User Create(string username, string passwordHash) => new()
    {
        Username = username,
        PasswordHash = passwordHash,
        CreatedAt = DateTime.UtcNow
    };
}
