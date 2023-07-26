using AutoMapper;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;

namespace Taktamir.Endpoint.Profiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateJobDto, Job>().ReverseMap();
            CreateMap<ReadJobDto, Job>().ReverseMap();
            CreateMap<CreateCustomerDto, Customer>().ReverseMap();
            CreateMap<ReadCustomerDto, Customer>().ReverseMap();
            CreateMap<ReadUserDto, User>().ReverseMap();
            CreateMap<ReadOrderDto, Order>().ReverseMap();
            CreateMap<ReadWalletDto, Wallet>().ReverseMap();
            CreateMap<ReadUserDto,User>().ReverseMap();
            CreateMap<UpdateUserDto,User>().ReverseMap();
            CreateMap<SpecialtyDto,Specialty>().ReverseMap();
            CreateMap<ReadUserDto,User>().ReverseMap();

            
        }
    }
}
