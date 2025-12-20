using AutoMapper;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Domain.Entities;

namespace FreelanceMarketplaceService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Project mappings
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.CompanyName))
                .ForMember(dest => dest.AssignedFreelancerName, opt => opt.MapFrom(src => src.AssignedFreelancer != null ? src.AssignedFreelancer.FullName : null))
                .ForMember(dest => dest.BidCount, opt => opt.MapFrom(src => src.Bids.Count));

            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();

            // Bid mappings
            CreateMap<Bid, BidResponseDto>()
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.FullName));

            CreateMap<CreateBidDto, Bid>();

            // Freelancer mappings
            CreateMap<Freelancer, FreelancerResponseDto>()
                .ForMember(dest => dest.PortfolioCount, opt => opt.MapFrom(src => src.Portfolio.Count));

            CreateMap<CreateFreelancerDto, Freelancer>();
            CreateMap<UpdateFreelancerDto, Freelancer>();

            // Portfolio mappings
            CreateMap<CreatePortfolioItemDto, PortfolioItem>();
        }
    }
}