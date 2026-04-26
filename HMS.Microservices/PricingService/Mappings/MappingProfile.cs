using AutoMapper;
using PricingService.Models;

namespace PricingService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Pricing, PricingDto>()
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType!.Name));
    }
}