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
      CreateMap<EnrichedPlayerDetails, PlayerDTO>();
      CreateMap<LevelProgressDetails, LevelProgressDTO>();
      CreateMap<WalletDetails, WalletDTO>();
      CreateMap<OrderDataDetails, OrderDataDTO>();
      CreateMap<StatsDetails, StatsDTO>();
      CreateMap<ItemSnapshot, ItemSnapshotDTO>();
      
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

      CreateMap<EnrichedShopSlotDetails, ShopSlotDTO>()
         .ForMember(
            dest => dest.SellerName,
            opt => opt.MapFrom(src => src.SellerName ?? "System"))
         ;
   }
}