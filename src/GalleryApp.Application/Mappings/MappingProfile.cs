using AutoMapper;
using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Entities;

namespace GalleryApp.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Picture mappings
        CreateMap<Picture, PictureDto>()
            .ForMember(dest => dest.GalleryTypeName, opt => opt.MapFrom(src => src.GalleryType != null ? src.GalleryType.Name : null));

        CreateMap<PictureCreateDto, Picture>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.GalleryType, opt => opt.Ignore());

        CreateMap<PictureUpdateDto, Picture>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.GalleryType, opt => opt.Ignore());

        // GalleryType mappings
        CreateMap<GalleryType, GalleryTypeDto>()
            .ForMember(dest => dest.PictureCount, opt => opt.MapFrom(src => src.Pictures.Count));

        CreateMap<GalleryTypeCreateDto, GalleryType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Pictures, opt => opt.Ignore());

        CreateMap<GalleryTypeUpdateDto, GalleryType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Pictures, opt => opt.Ignore());
    }
}
