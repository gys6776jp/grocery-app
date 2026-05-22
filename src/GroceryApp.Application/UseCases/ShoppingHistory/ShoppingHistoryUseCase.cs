using GroceryApp.Application.DTOs;
using GroceryApp.Domain.Entities;
using GroceryApp.Domain.Repositories;

namespace GroceryApp.Application.UseCases.ShoppingHistory;

public class ShoppingHistoryUseCase(IShoppingHistoryRepository historyRepo, IShoppingListRepository listRepo)
{
    public async Task<IEnumerable<ShoppingHistoryResponse>> GetAllAsync(int userId)
    {
        var histories = await historyRepo.GetByUserIdAsync(userId);
        return histories.Select(h => new ShoppingHistoryResponse(
            h.Id,
            h.CompletedAt,
            h.Items.Select(i => new ShoppingHistoryItemResponse(
                i.Id,
                i.Name,
                i.Quantity,
                i.Memo,
                i.IsChecked
            )).ToList()
        ));
    }

    public async Task<ShoppingHistoryResponse> CreateAsync(int userId, CreateShoppingHistoryRequest req)
    {
        var history = Domain.Entities.ShoppingHistory.Create(userId);
        foreach (var item in req.Items)
        {
            var historyItem = Domain.Entities.ShoppingHistoryItem.Create(
                history.Id,
                item.Name,
                item.Quantity,
                item.Memo,
                item.IsChecked
            );
            history.Items.Add(historyItem);
        }
        await historyRepo.AddAsync(history);
        await historyRepo.SaveChangesAsync();
        return new ShoppingHistoryResponse(
            history.Id,
            history.CompletedAt,
            history.Items.Select(i => new ShoppingHistoryItemResponse(
                i.Id,
                i.Name,
                i.Quantity,
                i.Memo,
                i.IsChecked
            )).ToList()
        );
    }

    public async Task RestoreAsync(int userId, int historyId)
    {
        var history = await historyRepo.GetByIdAsync(historyId)
            ?? throw new KeyNotFoundException("履歴が見つかりません");
        if (history.UserId != userId)
            throw new UnauthorizedAccessException("この履歴を復元する権限がありません");

        var list = await listRepo.GetByUserIdAsync(userId);
        if (list == null)
        {
            list = Domain.Entities.ShoppingList.Create(userId);
            await listRepo.AddListAsync(list);
            await listRepo.SaveChangesAsync();
            list = await listRepo.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException("リストの作成に失敗しました");
        }

        foreach (var historyItem in history.Items)
        {
            var item = Domain.Entities.ShoppingItem.Create(
                list.Id,
                historyItem.Name,
                historyItem.Quantity,
                historyItem.Memo
            );
            await listRepo.AddItemAsync(item);
        }
        await listRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int historyId)
    {
        var history = await historyRepo.GetByIdAsync(historyId)
            ?? throw new KeyNotFoundException("履歴が見つかりません");
        if (history.UserId != userId)
            throw new UnauthorizedAccessException("この履歴を削除する権限がありません");
        await historyRepo.RemoveAsync(history);
        await historyRepo.SaveChangesAsync();
    }
}
