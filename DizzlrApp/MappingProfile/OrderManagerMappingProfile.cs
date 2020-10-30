using AutoMapper;
using DizzlrApp.Models;
using DizzlrApp.ViewModels;

namespace DizzlrApp.MappingProfile
{
    public class OrderManagerMappingProfile : Profile
    {
        public OrderManagerMappingProfile()
        {

            CreateMap<Order, OrderManagerDetailVM>()
                    .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.OrderId))
                    .ForMember(d => d.OrderDate, opt => opt.MapFrom(s => s.OrderDate))
                    .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.Name))
                    .ForMember(d => d.TotalFiles, opt => opt.MapFrom(s => s.Files.Count))
                    .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName));
        }
    }
}
