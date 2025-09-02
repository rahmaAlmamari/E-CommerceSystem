using E_CommerceSystem.Models;
using AutoMapper;

namespace E_CommerceSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User <-> UserDTO ...
            CreateMap<User, UserDTO>().ReverseMap();

            // Product <-> ProductDTO ...
            CreateMap<Product, ProductDTO>().ReverseMap();

            // Review <-> ReviewDTO ...
            CreateMap<Review, ReviewDTO>().ReverseMap();

            // OrderProducts -> OrderItemDTO ...
            CreateMap<OrderProducts, OrderItemDTO>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.product.ProductName))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));

            // OrderItemDTO -> OrderProducts (single definition) ...
            CreateMap<OrderItemDTO, OrderProducts>()
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.product, o => o.Ignore())
                .ForMember(d => d.PID, o => o.Ignore())
                .ForMember(d => d.OID, o => o.Ignore())
                .ForMember(d => d.Order, o => o.Ignore());
            // both PID/OID will be set in service/controller ...

            // OrderProducts -> OrdersOutputOTD (per-item total) ...
            CreateMap<OrderProducts, OrdersOutputOTD>()
                .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.Order.OrderDate))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.product.ProductName))
                .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.Quantity * s.product.Price))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));
            // No ReverseMap() — output-only DTO ...
        }
    }
}
