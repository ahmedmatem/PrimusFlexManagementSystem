using AutoMapper;

namespace PrimusFlex.WebServices.Infrastructure.Mapping
{

    public interface IHaveCustomMappings
    {
        void CreateMappings(MapperConfiguration configuration);
    }
}
