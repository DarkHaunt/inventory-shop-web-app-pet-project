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
      
      // Web
      CreateMap<AggregatedPlayerDetails, PlayerDTO>();
      CreateMap<LevelProgressDetails, LevelProgressDTO>();
      CreateMap<WalletDetails, WalletDTO>();
      CreateMap<StatsDetails, StatsDTO>();
      
      CreateMap<EnrichedItemDetails, ItemDTO>()
         .ForMember(
            dest => dest.OwnerName,
            opt => opt.MapFrom(src => src.OwnerName ?? "None"))
         .ForMember(
            dest => dest.CreatorName,
            opt => opt.MapFrom(src => src.CreatorName ?? "System"))
         ;
   }
}