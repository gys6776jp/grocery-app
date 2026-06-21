using System.Net.Http.Headers;
using System.Net.Http.Json;
using GroceryApp.Models;
using Microsoft.JSInterop;

namespace GroceryApp.Services;

public class ApiService(HttpClient http, IJSRuntime js)
{
    private const string TokenKey = "auth_token";
    public event Action? OnUnauthorized;

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        return !string.IsNullOrEmpty(token);
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task HandleResponseAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("[ApiService] 401 Unauthorized detected, logging out...");
            await LogoutAsync();
            OnUnauthorized?.Invoke();
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var res = await http.PostAsJsonAsync("api/auth/login", request);
        if (!res.IsSuccessStatusCode) return null;
        var data = await res.Content.ReadFromJsonAsync<AuthResponse>();
        if (data != null)
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, data.Token);
        return data;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var res = await http.PostAsJsonAsync("api/auth/register", request);
        if (!res.IsSuccessStatusCode) return null;
        var data = await res.Content.ReadFromJsonAsync<AuthResponse>();
        if (data != null)
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, data.Token);
        return data;
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<ShoppingListResponse?> GetListAsync()
    {
        await SetAuthHeaderAsync();
        var res = await http.GetAsync("api/shoppinglist");
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ShoppingListResponse>() : null;
    }

    public async Task<ShoppingItemModel?> AddItemAsync(AddItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsJsonAsync("api/shoppinglist/items", request);
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<ShoppingItemModel>()
            : null;
    }

    public async Task ToggleItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        var res = await http.PatchAsync($"api/shoppinglist/items/{itemId}/toggle", null);
        await HandleResponseAsync(res);
    }

    public async Task UpdateItemAsync(int itemId, UpdateItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PutAsJsonAsync($"api/shoppinglist/items/{itemId}", request);
        await HandleResponseAsync(res);
    }

    public async Task DeleteItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        var res = await http.DeleteAsync($"api/shoppinglist/items/{itemId}");
        await HandleResponseAsync(res);
    }

    public async Task ResetCheckedAsync()
    {
        await SetAuthHeaderAsync();
        var res = await http.DeleteAsync("api/shoppinglist/reset");
        await HandleResponseAsync(res);
    }

    public async Task DeleteAllItemsAsync()
    {
        await SetAuthHeaderAsync();
        var res = await http.DeleteAsync("api/shoppinglist/all");
        await HandleResponseAsync(res);
    }

    public async Task<List<MasterItemModel>?> GetMasterItemsAsync()
    {
        await SetAuthHeaderAsync();
        var res = await http.GetAsync("api/masteritems");
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<MasterItemModel>>() : null;
    }

    public async Task<MasterItemModel?> AddMasterItemAsync(AddMasterItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsJsonAsync("api/masteritems", request);
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<MasterItemModel>()
            : null;
    }

    public async Task UpdateMasterItemAsync(int itemId, UpdateMasterItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PutAsJsonAsync($"api/masteritems/{itemId}", request);
        await HandleResponseAsync(res);
    }

    public async Task DeleteMasterItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        var res = await http.DeleteAsync($"api/masteritems/{itemId}");
        await HandleResponseAsync(res);
    }

    public async Task<List<ShoppingHistoryModel>?> GetShoppingHistoriesAsync()
    {
        await SetAuthHeaderAsync();
        var res = await http.GetAsync("api/shoppinghistories");
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<ShoppingHistoryModel>>() : null;
    }

    public async Task<ShoppingHistoryModel?> CreateShoppingHistoryAsync(CreateShoppingHistoryRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsJsonAsync("api/shoppinghistories", request);
        await HandleResponseAsync(res);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<ShoppingHistoryModel>()
            : null;
    }

    public async Task RestoreShoppingHistoryAsync(int historyId)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsync($"api/shoppinghistories/{historyId}/restore", null);
        await HandleResponseAsync(res);
    }

    public async Task DeleteShoppingHistoryAsync(int historyId)
    {
        await SetAuthHeaderAsync();
        var res = await http.DeleteAsync($"api/shoppinghistories/{historyId}");
        await HandleResponseAsync(res);
    }
}
