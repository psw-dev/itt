using AutoMapper;

namespace PSW.ITT.Service.AutoMapper
{
    public class EntityToDTOMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EntityToDTOMappings"; }
        }
        public EntityToDTOMappingProfile()
        {
            // GetCityByCountryResponseDTO
            //CreateMap<City, GetCityByCountryResponseDTO>()
            //    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Name));
        }
    }
}