using GroceryApp.Domain.Entities;

namespace GroceryApp.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}

public interface IShoppingListRepository
{
    Task<ShoppingList?> GetByUserIdAsync(int userId);
    Task<ShoppingItem?> GetItemByIdAsync(int itemId);
    Task AddListAsync(ShoppingList list);
    Task AddItemAsync(ShoppingItem item);
    Task RemoveItemAsync(ShoppingItem item);
    Task RemoveCheckedItemsAsync(int listId);
    Task RemoveAllItemsAsync(int listId);
    Task SaveChangesAsync();
}

public interface IMasterItemRepository
{
    Task<IEnumerable<MasterItem>> GetByUserIdAsync(int userId);
    Task<MasterItem?> GetByIdAsync(int id);
    Task AddAsync(MasterItem item);
    Task RemoveAsync(MasterItem item);
    Task SaveChangesAsync();
}

public interface IShoppingHistoryRepository
{
    Task<IEnumerable<ShoppingHistory>> GetByUserIdAsync(int userId);
    Task<ShoppingHistory?> GetByIdAsync(int id);
    Task AddAsync(ShoppingHistory history);
    Task RemoveAsync(ShoppingHistory history);
    Task SaveChangesAsync();
}
