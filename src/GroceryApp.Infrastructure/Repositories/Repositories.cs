using GroceryApp.Domain.Entities;
using GroceryApp.Domain.Repositories;
using GroceryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GroceryApp.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username) =>
        db.Users.FirstOrDefaultAsync(u => u.Username == username);
    public Task<User?> GetByIdAsync(int id) =>
        db.Users.FindAsync(id).AsTask();
    public async Task AddAsync(User user) => await db.Users.AddAsync(user);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

public class ShoppingListRepository(AppDbContext db) : IShoppingListRepository
{
    public Task<ShoppingList?> GetByUserIdAsync(int userId) =>
        db.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.UserId == userId);
    public Task<ShoppingItem?> GetItemByIdAsync(int itemId) =>
        db.ShoppingItems.FindAsync(itemId).AsTask();
    public async Task AddListAsync(ShoppingList list) => await db.ShoppingLists.AddAsync(list);
    public async Task AddItemAsync(ShoppingItem item) => await db.ShoppingItems.AddAsync(item);
    public Task RemoveItemAsync(ShoppingItem item) { db.ShoppingItems.Remove(item); return Task.CompletedTask; }
    public async Task RemoveCheckedItemsAsync(int listId)
    {
        var items = await db.ShoppingItems.Where(i => i.ListId == listId && i.IsChecked).ToListAsync();
        db.ShoppingItems.RemoveRange(items);
    }
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

public class MasterItemRepository(AppDbContext db) : IMasterItemRepository
{
    public async Task<IEnumerable<MasterItem>> GetByUserIdAsync(int userId) =>
        await db.MasterItems.Where(m => m.UserId == userId).ToListAsync();
    public Task<MasterItem?> GetByIdAsync(int id) =>
        db.MasterItems.FindAsync(id).AsTask();
    public async Task AddAsync(MasterItem item) => await db.MasterItems.AddAsync(item);
    public Task RemoveAsync(MasterItem item) { db.MasterItems.Remove(item); return Task.CompletedTask; }
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

public class ShoppingHistoryRepository(AppDbContext db) : IShoppingHistoryRepository
{
    public async Task<IEnumerable<ShoppingHistory>> GetByUserIdAsync(int userId) =>
        await db.ShoppingHistories.Include(h => h.Items).Where(h => h.UserId == userId).OrderByDescending(h => h.CompletedAt).ToListAsync();
    public Task<ShoppingHistory?> GetByIdAsync(int id) =>
        db.ShoppingHistories.Include(h => h.Items).FirstOrDefaultAsync(h => h.Id == id);
    public async Task AddAsync(ShoppingHistory history) => await db.ShoppingHistories.AddAsync(history);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
