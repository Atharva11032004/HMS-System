using AutoMapper;
using BillingService.Models;

namespace BillingService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bill, BillDto>();
        CreateMap<BillLine, BillLineDto>();
        CreateMap<Payment, PaymentDto>();
    }
}