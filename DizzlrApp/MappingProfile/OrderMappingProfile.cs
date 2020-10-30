using AutoMapper;
using DizzlrApp.Models;
using DizzlrApp.ViewModels;

namespace DizzlrApp.MappingProfile
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDetailVM>()
                .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.OrderId))
                .ForMember(d => d.OrderDate, opt => opt.MapFrom(s => s.OrderDate))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.Name))
                .ForMember(d => d.TotalFiles, opt => opt.MapFrom(s => s.Files.Count));
        }
    }
}
