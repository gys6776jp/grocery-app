namespace GroceryApp.Application.DTOs;

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password, string RegisterCode);
public record AuthResponse(string Token, string Username);
public record ShoppingListResponse(int Id, string Name, List<ShoppingItemResponse> Items);
public record ShoppingItemResponse(int Id, string Name, string? Quantity, string? Memo, bool IsChecked);
public record AddItemRequest(string Name, string? Quantity, string? Memo, bool SaveToMaster);
public record UpdateItemRequest(string Name, string? Quantity, string? Memo);
public record MasterItemResponse(int Id, string Name);
public record AddMasterItemRequest(string Name);
