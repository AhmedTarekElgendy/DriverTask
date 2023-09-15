using AutoMapper;
using Driver.Business.DTO;
using Driver.Business.Interfaces;
using Driver.DAL.Repository;
using Microsoft.Extensions.Logging;
using System;
using DriverModel = Driver.Domain.Driver;

namespace Driver.Business.Services
{
    public class DriverService : IDriverService
    {
        private readonly ILogger<DriverService> _logger;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;

        public DriverService(ILogger<DriverService> logger, IDriverRepository driverRepository,
            IMapper mapper)
        {
            _logger = logger;
            _driverRepository = driverRepository;
            _mapper = mapper;
        }
        public async Task<DriverDto> AddDriver(DriverDto driverDto)
        {
            var addnewDriverResponse = await _driverRepository.InsertNewDriver(_mapper.Map<DriverModel>(driverDto));
            if (addnewDriverResponse) { return driverDto; }
            return null;
        }

        public async Task<List<DriverDto>> AddDriverBulk()
        {
            var driverBulk = BuildDriverBulkItems();
            var addnewDriverResponse = await _driverRepository.InsertDriverBulk(driverBulk);
            if (addnewDriverResponse) { return _mapper.Map<List<DriverDto>>(driverBulk); }
            return null;
        }

        public async Task<(bool?, DriverDto)> UpdateDriver(int driverId, DriverDto driverDto)
        {
            var driver = await _driverRepository.GetDriver(driverId);
            if (driver == null)
                return (null, null);

            var updateDriverResponse = await _driverRepository.UpdateDriver(driverId, _mapper.Map<DriverModel>(driverDto));
            if (updateDriverResponse) { return (true, driverDto); }

            return (false, null);
        }
        public async Task<DriverDto> GetDriver(int driverId)
        {
            var driver = await _driverRepository.GetDriver(driverId);
            if (driver == null)
                return null;
            return _mapper.Map<DriverDto>(driver);
        }
        
         public async Task<List<string>> GetALLCapitalizedDrivers()
        {
            return await _driverRepository.GetALLCapitalizedDrivers();
        }

        public async Task<bool?> DeleteDriver(int driverId)
        {
            var driver = await _driverRepository.GetDriver(driverId);
            if (driver == null)
                return null;

            return await _driverRepository.DeleteDriver(driverId);
        }

        private List<DriverModel> BuildDriverBulkItems()
        {
            var drivers = new List<DriverModel>();
            for (int i = 0; i < 10; i++)
            {
                drivers.Add(new DriverModel
                {
                    FirstName = GenerateRandomString(),
                    LastName = GenerateRandomString(),
                    PhoneNumber = GenerateRandomPhone(),
                    Email = GenerateRandomString() + "@gmail.com"
                });
            }
            return drivers;
        }
        private string GenerateRandomString()
        {
            var random = new Random();

            const string chars = "ABCabc";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GenerateRandomPhone()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(
                Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
