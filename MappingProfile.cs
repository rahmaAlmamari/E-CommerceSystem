using E_CommerceSystem.Models;
using AutoMapper;

namespace E_CommerceSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User <-> UserDTO
            CreateMap<User, UserDTO>().ReverseMap();

            // Product <-> ProductDTO
            CreateMap<Product, ProductDTO>().ReverseMap();

            // Review <-> ReviewDTO
            CreateMap<Review, ReviewDTO>().ReverseMap();

            // OrderProducts -> OrderItemDTO
            CreateMap<OrderProducts, OrderItemDTO>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.product.ProductName))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity));

            // OrderItemDTO -> OrderProducts ...
            CreateMap<OrderItemDTO, OrderProducts>()
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.product,
                           opt => opt.Ignore()) 
                .ForMember(dest => dest.PID,
                           opt => opt.Ignore())
                .ForMember(dest => dest.OID,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Order,
                           opt => opt.Ignore());
            //use .Ignore() => tell AutoMapper Don’t try to map these, I’ll set them manually in my service/ controller ...

            // OrderProducts -> OrdersOutputOTD
            CreateMap<OrderProducts, OrdersOutputOTD>()
                .ForMember(dest => dest.OrderDate,
                           opt => opt.MapFrom(src => src.Order.OrderDate))
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src => src.Order.TotalAmount))
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.product.ProductName))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity));
            //we do not need ReverseMap() here because we won't map OrdersOutputOTD back to OrderProducts ...
            // OrdersOutputOTD is only used for output purposes ...

        }
    }
}
