namespace GroceryApp.Models;

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password, string RegisterCode);
public record AuthResponse(string Token, string Username);
public record ShoppingListResponse(int Id, string Name, List<ShoppingItemModel> Items);
public record ShoppingItemModel(int Id, string Name, string? Quantity, string? Memo, bool IsChecked);
public record AddItemRequest(string Name, string? Quantity, string? Memo, bool SaveToMaster);
public record UpdateItemRequest(string Name, string? Quantity, string? Memo);
public record MasterItemModel(int Id, string Name, string? Memo);
public record AddMasterItemRequest(string Name, string? Memo);
public record UpdateMasterItemRequest(string Name, string? Memo);
public record ShoppingHistoryModel(int Id, DateTime CompletedAt, List<ShoppingHistoryItemModel> Items);
public record ShoppingHistoryItemModel(int Id, string Name, string? Quantity, string? Memo, bool IsChecked);
public record CreateShoppingHistoryRequest(List<ShoppingItemSnapshot> Items);
public record ShoppingItemSnapshot(string Name, string? Quantity, string? Memo, bool IsChecked);
