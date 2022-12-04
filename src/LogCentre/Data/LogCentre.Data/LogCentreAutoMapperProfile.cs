using AutoMapper;

using LogCentre.Data.Entities;
using LogCentre.Model;

namespace LogCentre.Data
{
    public class LogCentreAutoMapperProfile : Profile
    {
        public LogCentreAutoMapperProfile()
        {
            CreateMap<Host, HostModel>().ReverseMap();
            CreateMap<Provider, ProviderModel>().ReverseMap();
            CreateMap<LogSource, LogSourceModel>().ReverseMap();
            CreateMap<Entities.Log.File, Model.Log.FileModel>().ReverseMap();
            CreateMap<Entities.Log.Line, Model.Log.LineModel>().ReverseMap();
        }
    }
}
