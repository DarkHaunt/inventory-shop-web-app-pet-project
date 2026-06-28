using AutoMapper;
using InventoryShop.Application.DTO;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Web.DTO;

namespace InventoryShop.Web.Profiles;

public sealed class MappingProfile : Profile
{
   public MappingProfile()
   {
      // Application
      CreateMap<LevelProgress, LevelProgressDetails>();
      CreateMap<Wallet, WalletDetails>();
      CreateMap<Stats, StatsDetails>();
      CreateMap<OrderData, OrderDataDetails>();
      
      // Web
      CreateMap<AggregatedPlayerDetails, PlayerDTO>();
      CreateMap<LevelProgressDetails, LevelProgressDTO>();
      CreateMap<WalletDetails, WalletDTO>();
      CreateMap<OrderDataDetails, OrderDataDTO>();
      CreateMap<StatsDetails, StatsDTO>();
      
      CreateMap<ItemInOrderSnapshot, ItemDTO>()
         .ForMember(
            dest => dest.Id,
            opt => opt.MapFrom(src => src.Id))
         .ForMember(
            dest => dest.Description,
            opt => opt.MapFrom(src => src.Description))
         .ForMember(
            dest => dest.StatsModifiers,
            opt => opt.MapFrom(src => src.StatsModifiers))
         .ForMember(
            dest => dest.Type,
            opt => opt.MapFrom(src => src.Type))
         ;
      
      // Web-Presentation
      CreateMap<EnrichedItemDetails, ItemDTO>()
         .ForMember(
            dest => dest.OwnerName,
            opt => opt.MapFrom(src => src.OwnerName ?? "None"))
         .ForMember(
            dest => dest.CreatorName,
            opt => opt.MapFrom(src => src.CreatorName ?? "System"))
         ;
      
      CreateMap<EnrichedShopOrderDetails, ShopOrderDTO>()
         .ForMember(
            dest => dest.SellerName,
            opt => opt.MapFrom(src => src.SellerName ?? "System"))
         ;
   }
}