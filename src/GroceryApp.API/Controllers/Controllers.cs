using System.Security.Claims;
using GroceryApp.Application.DTOs;
using GroceryApp.Application.UseCases.Auth;
using GroceryApp.Application.UseCases.Master;
using GroceryApp.Application.UseCases.ShoppingList;
using GroceryApp.Application.UseCases.ShoppingHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GroceryApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthUseCase authUseCase, IConfiguration config) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try { return Ok(await authUseCase.LoginAsync(request)); }
        catch (UnauthorizedAccessException) { return Unauthorized(new { message = "ユーザー名またはパスワードが正しくありません" }); }
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var validCode = config["Auth:RegisterCode"] ?? string.Empty;
            return Ok(await authUseCase.RegisterAsync(request, validCode));
        }
        catch (UnauthorizedAccessException) { return Unauthorized(new { message = "登録できませんでした" }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShoppingListController(ShoppingListUseCase useCase) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetList() => Ok(await useCase.GetOrCreateListAsync(UserId));

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        try { return Ok(await useCase.AddItemAsync(UserId, request)); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("items/{itemId:int}/toggle")]
    public async Task<IActionResult> Toggle(int itemId)
    {
        try { await useCase.ToggleItemAsync(UserId, itemId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPut("items/{itemId:int}")]
    public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateItemRequest request)
    {
        try { await useCase.UpdateItemAsync(UserId, itemId, request); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> DeleteItem(int itemId)
    {
        try { await useCase.DeleteItemAsync(UserId, itemId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("reset")]
    public async Task<IActionResult> ResetChecked()
    {
        await useCase.ResetCheckedAsync(UserId);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MasterItemsController(MasterItemUseCase useCase) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await useCase.GetAllAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddMasterItemRequest request)
    {
        try { return Ok(await useCase.AddAsync(UserId, request)); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{itemId:int}")]
    public async Task<IActionResult> Update(int itemId, [FromBody] UpdateMasterItemRequest request)
    {
        try { await useCase.UpdateAsync(UserId, itemId, request); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
    }

    [HttpDelete("{itemId:int}")]
    public async Task<IActionResult> Delete(int itemId)
    {
        try { await useCase.DeleteAsync(UserId, itemId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShoppingHistoriesController(ShoppingHistoryUseCase useCase) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await useCase.GetAllAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShoppingHistoryRequest request)
    {
        try { return Ok(await useCase.CreateAsync(UserId, request)); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("{historyId:int}/restore")]
    public async Task<IActionResult> Restore(int historyId)
    {
        try { await useCase.RestoreAsync(UserId, historyId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
    }
}
