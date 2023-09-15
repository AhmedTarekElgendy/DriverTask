using Driver.Business.DTO;
using Driver.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Driver.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ILogger<DriverController> _logger;
        private readonly IDriverService _driverService;

        public DriverController(ILogger<DriverController> logger, IDriverService driverService)
        {
            _logger = logger;
            _driverService = driverService;
        }

        [HttpPost("AddDriver")]
        public async Task<IActionResult> AddDriver(DriverDto driverDto)
        {
            try
            {
                _logger.LogInformation($"Add driver request is {JsonConvert.SerializeObject(driverDto)}");

                if (!ModelState.IsValid)
                {
                    return HandleStatusCode(400, "Your request is not valid");
                }
                var addedDriver = await _driverService.AddDriver(driverDto);
                if (addedDriver == null)
                    return HandleStatusCode(500, "Error while adding new driver");

                return Ok(addedDriver);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding new driver with message {ex.Message}");
                return HandleStatusCode(500, "Error while adding new driver");
            }
        }

        [HttpPost("AddDriverBulk")]
        public async Task<IActionResult> AddDriverBulk()
        {
            try
            {
                var addedDriver = await _driverService.AddDriverBulk();

                if (addedDriver == null)
                    return HandleStatusCode(500, "Error while adding driver bulk");

                return Ok(addedDriver);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding driver bulk with message {ex.Message}");
                return HandleStatusCode(500, "Error while adding driver bulk");
            }
        }

        [HttpPut("UpdateDriver/{driverId}")]
        public async Task<IActionResult> UpdateDriver(int driverId, DriverDto driverDto)
        {
            try
            {
                _logger.LogInformation($"Update driver request is {JsonConvert.SerializeObject(driverDto)}");

                if (!ModelState.IsValid)
                {
                    return HandleStatusCode(400, "Your request is not valid");
                }
                var updateDriverRes = await _driverService.UpdateDriver(driverId, driverDto);

                if (updateDriverRes.Item1 == null)
                    return HandleStatusCode(404, "Driver not existed");

                else if (updateDriverRes.Item1 == false)
                    return HandleStatusCode(500, "Error while updating a driver");

                return Ok(updateDriverRes.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating driver with message {ex.Message}");
                return HandleStatusCode(500, "Error while updating driver");
            }
        }


        [HttpGet("GetDriverById/{driverId}")]
        public async Task<IActionResult> GetDriverById(int driverId)
        {
            try
            {
                var driver = await _driverService.GetDriver(driverId);

                if (driver == null)
                    return HandleStatusCode(404, "Driver not existed");

                return Ok(driver);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting a driver with message {ex.Message}");
                return HandleStatusCode(500, "Error while getting a driver");
            }
        }

        [HttpGet("GettAllCapitalizedDrivers")]
        public async Task<IActionResult> GetDriverById()
        {
            try
            {
                var drivers = await _driverService.GetALLCapitalizedDrivers();

                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting drivers with message {ex.Message}");
                return HandleStatusCode(500, "Error while getting drivers");
            }
        }

        [HttpDelete("DeleteDriverById/{driverId}")]
        public async Task<IActionResult> DeleteDriverById(int driverId)
        {
            try
            {
                var isDeleted = await _driverService.DeleteDriver(driverId);

                if (isDeleted == null)
                    return HandleStatusCode(404, "Driver not existed");

                else if (isDeleted != true)
                    return HandleStatusCode(500, "Error while deleting a driver");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting a driver with message {ex.Message}");
                return HandleStatusCode(500, "Error while deleting a driver");
            }
        }

        private ObjectResult HandleStatusCode(int statusCode, string message) =>
            StatusCode(statusCode, message);
    }
}