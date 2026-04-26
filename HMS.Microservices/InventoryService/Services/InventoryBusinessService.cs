using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class InventoryBusinessService
{
    private readonly ApplicationDbContext _context;

    public InventoryBusinessService(ApplicationDbContext context)
    {
        _context = context;
    }


    // getting all the items 
    public async Task<IEnumerable<InventoryItemDto>> GetAllItemsAsync()
    {
        var items = await _context.InventoryItems.ToListAsync();
        return items.Select(i => new InventoryItemDto
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            Category = i.Category,
            Supplier = i.Supplier,
            QuantityInStock = i.QuantityInStock,
            MinimumStockLevel = i.MinimumStockLevel,
            UnitPrice = i.UnitPrice,
            LastRestocked = i.LastRestocked,
            IsActive = i.IsActive
        });
    }
    
    // getting the items by id 
    public async Task<InventoryItemDto?> GetItemByIdAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null) return null;

        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Category = item.Category,
            Supplier = item.Supplier,
            QuantityInStock = item.QuantityInStock,
            MinimumStockLevel = item.MinimumStockLevel,
            UnitPrice = item.UnitPrice,
            LastRestocked = item.LastRestocked,
            IsActive = item.IsActive
        };
    }
    

    // creating the item 
    public async Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request)
    {
        var item = new InventoryItem
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Supplier = request.Supplier,
            QuantityInStock = request.QuantityInStock,
            MinimumStockLevel = request.MinimumStockLevel,
            UnitPrice = request.UnitPrice,
            LastRestocked = DateTime.UtcNow,
            IsActive = true
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return await GetItemByIdAsync(item.Id) ?? throw new InvalidOperationException("Failed to create item");
    }

    // update item by id 
    public async Task<InventoryItemDto?> UpdateItemAsync(int id, UpdateInventoryItemRequest request)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null) return null;

        item.Name = request.Name;
        item.Description = request.Description;
        item.Category = request.Category;
        item.Supplier = request.Supplier;
        item.MinimumStockLevel = request.MinimumStockLevel;
        item.UnitPrice = request.UnitPrice;
        item.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return await GetItemByIdAsync(id);
    }
    
    // delete the item by id 
    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null) return false;

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
    
    // adjusting the stock by id 
    public async Task<InventoryItemDto?> AdjustStockAsync(int id, AdjustStockRequest request)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null) return null;

        item.QuantityInStock += request.QuantityAdjustment;
        if (item.QuantityInStock < 0) item.QuantityInStock = 0; 

        if (request.QuantityAdjustment > 0)
        {
            item.LastRestocked = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetItemByIdAsync(id);
    }
    
    // getting the low stock items 
    public async Task<IEnumerable<InventoryItemDto>> GetLowStockItemsAsync()
    {
        var items = await _context.InventoryItems
            .Where(i => i.QuantityInStock <= i.MinimumStockLevel && i.IsActive)
            .ToListAsync();

        return items.Select(i => new InventoryItemDto
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            Category = i.Category,
            Supplier = i.Supplier,
            QuantityInStock = i.QuantityInStock,
            MinimumStockLevel = i.MinimumStockLevel,
            UnitPrice = i.UnitPrice,
            LastRestocked = i.LastRestocked,
            IsActive = i.IsActive
        });
    }
}