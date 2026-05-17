using GroceryApp.Application.DTOs;
using GroceryApp.Domain.Entities;
using GroceryApp.Domain.Repositories;

namespace GroceryApp.Application.UseCases.Master;

public class MasterItemUseCase(IMasterItemRepository masterRepo)
{
    public async Task<IEnumerable<MasterItemResponse>> GetAllAsync(int userId)
    {
        var items = await masterRepo.GetByUserIdAsync(userId);
        return items.Select(m => new MasterItemResponse(m.Id, m.Name));
    }

    public async Task<MasterItemResponse> AddAsync(int userId, AddMasterItemRequest req)
    {
        var existing = await masterRepo.GetByUserIdAsync(userId);
        if (existing.Any(m => m.Name == req.Name))
            throw new InvalidOperationException("同じ名前の商品がすでに登録されています");
        var item = MasterItem.Create(userId, req.Name);
        await masterRepo.AddAsync(item);
        await masterRepo.SaveChangesAsync();
        return new MasterItemResponse(item.Id, item.Name);
    }

    public async Task DeleteAsync(int userId, int itemId)
    {
        var item = await masterRepo.GetByIdAsync(itemId)
            ?? throw new KeyNotFoundException("商品が見つかりません");
        await masterRepo.RemoveAsync(item);
        await masterRepo.SaveChangesAsync();
    }
}
