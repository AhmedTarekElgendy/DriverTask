using Driver.Business.DTO;

namespace Driver.Business.Interfaces
{
    public interface IDriverService
    {
        Task<DriverDto> AddDriver(DriverDto DriverDto);
        Task<(bool?, DriverDto)> UpdateDriver(int driverId, DriverDto DriverDto);
        Task<DriverDto> GetDriver(int driverId);
        Task<bool?> DeleteDriver(int driverId);
        Task<List<DriverDto>> AddDriverBulk();
        Task<List<string>> GetALLCapitalizedDrivers();
    }
}
