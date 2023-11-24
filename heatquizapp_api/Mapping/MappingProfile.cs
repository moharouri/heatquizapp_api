using AutoMapper;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;

namespace HeatQuizAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {


            //Base entity
            CreateMap<BaseEntity, BaseEntityViewModel>();

            //Datapools
            CreateMap<DataPool, DataPoolViewModel>();
            CreateMap<DataPool, DataPoolViewModelAdmin>();

            CreateMap<DataPoolAccess, DataPoolAccessViewModel>();

        }
    }
}
