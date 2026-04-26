using AutoMapper;
using ReservationService.Models;

namespace ReservationService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => $"Guest {src.GuestId}"))
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RoomNumber));

        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType!.Name));
    }
}