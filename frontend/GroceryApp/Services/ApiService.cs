using System.Net.Http.Headers;
using System.Net.Http.Json;
using GroceryApp.Models;
using Microsoft.JSInterop;

namespace GroceryApp.Services;

public class ApiService(HttpClient http, IJSRuntime js)
{
    private const string TokenKey = "auth_token";

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
        return await http.GetFromJsonAsync<ShoppingListResponse>("api/shoppinglist");
    }

    public async Task<ShoppingItemModel?> AddItemAsync(AddItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsJsonAsync("api/shoppinglist/items", request);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<ShoppingItemModel>()
            : null;
    }

    public async Task ToggleItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        await http.PatchAsync($"api/shoppinglist/items/{itemId}/toggle", null);
    }

    public async Task DeleteItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        await http.DeleteAsync($"api/shoppinglist/items/{itemId}");
    }

    public async Task ResetCheckedAsync()
    {
        await SetAuthHeaderAsync();
        await http.DeleteAsync("api/shoppinglist/reset");
    }

    public async Task<List<MasterItemModel>?> GetMasterItemsAsync()
    {
        await SetAuthHeaderAsync();
        return await http.GetFromJsonAsync<List<MasterItemModel>>("api/masteritems");
    }

    public async Task<MasterItemModel?> AddMasterItemAsync(AddMasterItemRequest request)
    {
        await SetAuthHeaderAsync();
        var res = await http.PostAsJsonAsync("api/masteritems", request);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<MasterItemModel>()
            : null;
    }

    public async Task DeleteMasterItemAsync(int itemId)
    {
        await SetAuthHeaderAsync();
        await http.DeleteAsync($"api/masteritems/{itemId}");
    }
}
