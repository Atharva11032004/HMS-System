using AutoMapper;
using RoomService.Models;

namespace RoomService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType!.Name));

        CreateMap<RoomType, RoomTypeDto>();
    } 
}