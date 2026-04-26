using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Models;
using InventoryService.Services;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly InventoryBusinessService _inventoryService;

    public InventoryController(InventoryBusinessService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    // get route 
    [HttpGet]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _inventoryService.GetAllItemsAsync();
        return Ok(new ApiResponse<IEnumerable<InventoryItemDto>>
        {
            Success = true,
            Message = "Inventory items retrieved successfully",
            Data = items
        });
    }
    

    // get route by id 
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetItem(int id)
    {
        var item = await _inventoryService.GetItemByIdAsync(id);
        if (item == null)
        {
            return NotFound(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Inventory item not found"
            });
        }

        return Ok(new ApiResponse<InventoryItemDto>
        {
            Success = true,
            Message = "Inventory item retrieved successfully",
            Data = item
        });
    }
    

    // post route for create item 
    [HttpPost]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> CreateItem([FromBody] CreateInventoryItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var item = await _inventoryService.CreateItemAsync(request);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ApiResponse<InventoryItemDto>
        {
            Success = true,
            Message = "Inventory item created successfully",
            Data = item
        });
    }
     

     // put route 
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateInventoryItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var item = await _inventoryService.UpdateItemAsync(id, request);
        if (item == null)
        {
            return NotFound(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Inventory item not found"
            });
        }

        return Ok(new ApiResponse<InventoryItemDto>
        {
            Success = true,
            Message = "Inventory item updated successfully",
            Data = item
        });
    }
    
    // delete route 
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var result = await _inventoryService.DeleteItemAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Inventory item not found"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Inventory item deleted successfully"
        });
    }
    

    // adjust stock route . 
    [HttpPost("{id}/adjust-stock")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var item = await _inventoryService.AdjustStockAsync(id, request);
        if (item == null)
        {
            return NotFound(new ApiResponse<InventoryItemDto>
            {
                Success = false,
                Message = "Inventory item not found"
            });
        }

        return Ok(new ApiResponse<InventoryItemDto>
        {
            Success = true,
            Message = "Stock adjusted successfully",
            Data = item
        });
    }
    

    // get route for low-stock 
    [HttpGet("low-stock")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var items = await _inventoryService.GetLowStockItemsAsync();
        return Ok(new ApiResponse<IEnumerable<InventoryItemDto>>
        {
            Success = true,
            Message = "Low stock items retrieved successfully",
            Data = items
        });
    }
}