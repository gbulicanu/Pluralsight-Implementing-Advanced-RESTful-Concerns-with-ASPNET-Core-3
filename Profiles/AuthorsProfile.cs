using AutoMapper;

using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(destination => destination.Name,
                options => options.MapFrom(source => $"{source.FirstName} {source.LastName}"))
                .ForMember(destination => destination.Age,
                options => options.MapFrom(source => source.DateOfBirth.GetCurrentAge()));

            CreateMap<Models.AuthorForCreateDto, Entities.Author>();
        }
    }
}
