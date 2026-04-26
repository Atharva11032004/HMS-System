using AutoMapper;
using GuestService.Models;

namespace GuestService.Mappings;


// mapping profile for automapper to map between guest and dto . 
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Guest, GuestDto>();
        CreateMap<GuestDto, Guest>();
    }
}