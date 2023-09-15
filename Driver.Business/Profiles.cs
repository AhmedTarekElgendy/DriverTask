using AutoMapper;
using Driver.Business.DTO;
using DriverModel = Driver.Domain.Driver;

namespace Driver.Business
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<DriverModel, DriverDto>().ReverseMap();
        }
    }
}
