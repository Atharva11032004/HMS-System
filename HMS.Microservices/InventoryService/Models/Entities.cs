using System.ComponentModel.DataAnnotations;

namespace InventoryService.Models;

public class InventoryItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Supplier { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MinimumStockLevel { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public DateTime LastRestocked { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}


public class InventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime LastRestocked { get; set; }
    public bool IsActive { get; set; }
    public bool IsLowStock => QuantityInStock <= MinimumStockLevel;
}

public class CreateInventoryItemRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Supplier { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MinimumStockLevel { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class UpdateInventoryItemRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Supplier { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int MinimumStockLevel { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public bool IsActive { get; set; } = true;
}

public class AdjustStockRequest
{
    [Required]
    [Range(int.MinValue, int.MaxValue)]
    public int QuantityAdjustment { get; set; } 

    public string? Reason { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}