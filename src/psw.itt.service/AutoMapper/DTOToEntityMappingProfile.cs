using System.Globalization;
using AutoMapper;

namespace PSW.ITT.Service.AutoMapper
{
    public class DTOToEntityMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DTOToEntityMappings"; }
        }
        public DTOToEntityMappingProfile()
        {
            var _culture = new CultureInfo("en-Us");

            //CreateMap<GetCityByCountryRequestDTO, Country>()
            //    .ForMember(dest => dest.Code , opt => opt.MapFrom(src => src.CountryCode));

        }
    }
}