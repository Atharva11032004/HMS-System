using AutoMapper;
using InventoryService.Models;

namespace InventoryService.Mappings;
 
// mapping profile for inventory service.  
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>();
        CreateMap<CreateInventoryItemRequest, InventoryItem>();
        CreateMap<UpdateInventoryItemRequest, InventoryItem>();
    }
}