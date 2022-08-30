using AutoMapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.AutoMapper
{

    public class ObjectMapper
    {
        public IMapper _mapper { get; set; }

        public ObjectMapper()
        {
            ConfigureMappings();
        }

        public IMapper GetMapper()
        {
            return _mapper;
        }

        public void ConfigureMappings()
        {
            try
            {
                // Place All Mappings Here

                // NewKeyDTO to ApiKey Mappings 
                var config = new MapperConfiguration(cfg =>
                {
                     cfg.CreateMap<ProductCodeEntity, FetchActiveProductCodesListResponseDTO>();

                });

                _mapper = config.CreateMapper();

            }
            catch
            {
                throw;
            }
        }

    }
}