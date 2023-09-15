using Driver.Domain;
using DriverModel = Driver.Domain.Driver;

namespace Driver.DAL.Repository
{
    public interface IDriverRepository
    {
        Task<bool> InsertNewDriver(DriverModel driver);
        Task<bool> UpdateDriver(int driverId, DriverModel driver);
        Task<DriverModel> GetDriver(int driverId);
        Task<bool> DeleteDriver(int driverId);
        Task<bool> InsertDriverBulk(List<DriverModel> driver);
        Task<List<string>> GetALLCapitalizedDrivers();
    }
}
