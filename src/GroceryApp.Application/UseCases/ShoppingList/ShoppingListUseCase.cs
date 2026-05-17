using GroceryApp.Application.DTOs;
using GroceryApp.Domain.Repositories;
using ListEntity = GroceryApp.Domain.Entities.ShoppingList;
using ItemEntity = GroceryApp.Domain.Entities.ShoppingItem;
using MasterEntity = GroceryApp.Domain.Entities.MasterItem;

namespace GroceryApp.Application.UseCases.ShoppingList;

public class ShoppingListUseCase(IShoppingListRepository listRepo, IMasterItemRepository masterRepo)
{
    public async Task<ShoppingListResponse> GetOrCreateListAsync(int userId)
    {
        var list = await listRepo.GetByUserIdAsync(userId);
        if (list == null)
        {
            list = ListEntity.Create(userId);
            await listRepo.AddListAsync(list);
            await listRepo.SaveChangesAsync();
            list = await listRepo.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException("リストの作成に失敗しました");
        }
        return ToResponse(list);
    }

    public async Task<ShoppingItemResponse> AddItemAsync(int userId, AddItemRequest req)
    {
        var list = await listRepo.GetByUserIdAsync(userId);
        if (list == null)
        {
            list = ListEntity.Create(userId);
            await listRepo.AddListAsync(list);
            await listRepo.SaveChangesAsync();
            list = await listRepo.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException("リストの作成に失敗しました");
        }
        var item = ItemEntity.Create(list.Id, req.Name, req.Quantity, req.Memo);
        await listRepo.AddItemAsync(item);
        if (req.SaveToMaster)
        {
            var masters = await masterRepo.GetByUserIdAsync(userId);
            if (!masters.Any(m => m.Name == req.Name))
                await masterRepo.AddAsync(MasterEntity.Create(userId, req.Name));
        }
        await listRepo.SaveChangesAsync();
        return ToItemResponse(item);
    }

    public async Task ToggleItemAsync(int userId, int itemId)
    {
        var item = await listRepo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("アイテムが見つかりません");
        item.ToggleCheck();
        await listRepo.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(int userId, int itemId, UpdateItemRequest req)
    {
        var item = await listRepo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("アイテムが見つかりません");
        item.Update(req.Name, req.Quantity, req.Memo);
        await listRepo.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int userId, int itemId)
    {
        var item = await listRepo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("アイテムが見つかりません");
        await listRepo.RemoveItemAsync(item);
        await listRepo.SaveChangesAsync();
    }

    public async Task ResetCheckedAsync(int userId)
    {
        var list = await listRepo.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("リストが見つかりません");
        await listRepo.RemoveCheckedItemsAsync(list.Id);
        await listRepo.SaveChangesAsync();
    }

    private static ShoppingListResponse ToResponse(ListEntity list) =>
        new(list.Id, list.Name, list.Items.Select(ToItemResponse).ToList());

    private static ShoppingItemResponse ToItemResponse(ItemEntity item) =>
        new(item.Id, item.Name, item.Quantity, item.Memo, item.IsChecked);
}
