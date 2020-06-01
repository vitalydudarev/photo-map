using AutoMapper;
using Storage.Service.Database.Entities;
using Storage.Service.Models;

namespace Storage.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IncomingFile, File>()
                .ForMember(a => a.Id, expression => expression.Ignore())
                .ForMember(a => a.FullPath, expression => expression.MapFrom(b => b.Path));

            CreateMap<File, OutgoingFile>();

            CreateMap<File, OutgoingFileInfo>();
        }
    }
}
