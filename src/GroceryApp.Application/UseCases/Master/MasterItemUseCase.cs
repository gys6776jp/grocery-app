using GroceryApp.Application.DTOs;
using GroceryApp.Domain.Entities;
using GroceryApp.Domain.Repositories;

namespace GroceryApp.Application.UseCases.Master;

public class MasterItemUseCase(IMasterItemRepository masterRepo)
{
    public async Task<IEnumerable<MasterItemResponse>> GetAllAsync(int userId)
    {
        var items = await masterRepo.GetByUserIdAsync(userId);
        return items.Select(m => new MasterItemResponse(m.Id, m.Name, m.Memo));
    }

    public async Task<MasterItemResponse> AddAsync(int userId, AddMasterItemRequest req)
    {
        var existing = await masterRepo.GetByUserIdAsync(userId);
        if (existing.Any(m => m.Name == req.Name))
            throw new InvalidOperationException("同じ名前の商品がすでに登録されています");
        var item = MasterItem.Create(userId, req.Name, req.Memo);
        await masterRepo.AddAsync(item);
        await masterRepo.SaveChangesAsync();
        return new MasterItemResponse(item.Id, item.Name, item.Memo);
    }

    public async Task UpdateAsync(int userId, int itemId, UpdateMasterItemRequest req)
    {
        var item = await masterRepo.GetByIdAsync(itemId)
            ?? throw new KeyNotFoundException("商品が見つかりません");
        if (item.UserId != userId)
            throw new UnauthorizedAccessException("この商品を編集する権限がありません");
        item.Update(req.Name, req.Memo);
        await masterRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int itemId)
    {
        var item = await masterRepo.GetByIdAsync(itemId)
            ?? throw new KeyNotFoundException("商品が見つかりません");
        if (item.UserId != userId)
            throw new UnauthorizedAccessException("この商品を削除する権限がありません");
        await masterRepo.RemoveAsync(item);
        await masterRepo.SaveChangesAsync();
    }
}
