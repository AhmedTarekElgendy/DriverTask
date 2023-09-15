using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SQLite;
using DriverModel = Driver.Domain.Driver;

namespace Driver.DAL.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverRepository> _logger;
        private string connectionString;
        private readonly string TableName = "driver";
        public DriverRepository(IConfiguration configuration, ILogger<DriverRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            connectionString = _configuration.GetConnectionString("DriverConnectionString");
        }
        public async Task<bool> InsertNewDriver(DriverModel driver)
        {
            try
            {
                var query = "INSERT INTO driver ('FirstName', 'LastName', 'Email', 'PhoneNumber') VALUES (@firstName, @lastName, @email, @phoneNumber)";

                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    sqlCommand.Parameters.AddWithValue("@firstName", driver.FirstName);
                    sqlCommand.Parameters.AddWithValue("@lastName", driver.LastName);
                    sqlCommand.Parameters.AddWithValue("@email", driver.Email);
                    sqlCommand.Parameters.AddWithValue("@phoneNumber", driver.PhoneNumber);
                    var response = await sqlCommand.ExecuteNonQueryAsync();
                    SC.Close();
                    return response == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while adding new driver with hmessage {ex.Message}");
            }
            return false;
        }

        public async Task<bool> UpdateDriver(int driverId, DriverModel driver)
        {
            try
            {
                var query = $"UPDATE {TableName} SET FirstName = @firstName, LastName = @lastName, Email = @email, PhoneNumber = @phoneNumber WHERE Id = @driverId";

                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    sqlCommand.Parameters.AddWithValue("@firstName", driver.FirstName);
                    sqlCommand.Parameters.AddWithValue("@lastName", driver.LastName);
                    sqlCommand.Parameters.AddWithValue("@email", driver.Email);
                    sqlCommand.Parameters.AddWithValue("@phoneNumber", driver.PhoneNumber);
                    sqlCommand.Parameters.AddWithValue("@driverId", driverId);

                    var response = await sqlCommand.ExecuteNonQueryAsync();
                    SC.Close();
                    return response == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while updating driver with hmessage {ex.Message}");
            }
            return false;
        }
        public async Task<bool> DeleteDriver(int driverId)
        {
            try
            {
                var query = $"DELETE FROM {TableName} WHERE Id = @driverId";

                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    sqlCommand.Parameters.AddWithValue("@driverId", driverId);
                    var response = await sqlCommand.ExecuteNonQueryAsync();
                    SC.Close();
                    return response == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while deleting a driver with hmessage {ex.Message}");
            }
            return false;
        }

        public async Task<DriverModel> GetDriver(int driverId)
        {
            try
            {
                var query = $"SELECT * FROM {TableName} WHERE Id = @driverId";

                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    sqlCommand.Parameters.AddWithValue("@driverId", driverId);
                    var response = sqlCommand.ExecuteReader();

                    if (!response.HasRows)
                        return null;

                    await response.ReadAsync();
                    var driver = PopulateDriverModel(response);
                    await response.CloseAsync();
                    await SC.CloseAsync();
                    return driver;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while Getting a driver with hmessage {ex.Message}");
            }
            return null;
        }

        public async Task<List<string>> GetALLCapitalizedDrivers()
        {
            try
            {
                var query = $"SELECT * FROM {TableName}";

                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    var response = sqlCommand.ExecuteReader();

                    if (!response.HasRows)
                        return null;

                    var drivers = await PopulateDriverList(response);
                    await response.CloseAsync();
                    await SC.CloseAsync();
                    return drivers;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while Getting a driver with hmessage {ex.Message}");
            }
            return null;
        }

        public async Task<bool> InsertDriverBulk(List<DriverModel> driver)
        {
            try
            {
                var query = "INSERT INTO driver ('FirstName', 'LastName', 'Email', 'PhoneNumber') " +
                    "VALUES (@firstName, @lastName, @email, @phoneNumber)";
                for (int i = 0; i < 9; i++)
                {
                    query += $" ,(@firstName{i + 1}, @lastName{i + 1}, @email{i + 1}, @phoneNumber{i + 1})";
                }
                using (var SC = new SQLiteConnection(connectionString))
                {
                    var sqlCommand = new SQLiteCommand(query, SC);
                    SC.Open();

                    sqlCommand.Parameters.AddWithValue("@firstName", driver[0].FirstName);
                    sqlCommand.Parameters.AddWithValue("@lastName", driver[0].LastName);
                    sqlCommand.Parameters.AddWithValue("@email", driver[0].Email);
                    sqlCommand.Parameters.AddWithValue("@phoneNumber", driver[0].PhoneNumber);
                    for (int i = 0; i < 9; i++)
                    {
                        sqlCommand.Parameters.AddWithValue($"@firstName{i + 1}", driver[i + 1].FirstName);
                        sqlCommand.Parameters.AddWithValue($"@lastName{i + 1}", driver[i + 1].LastName);
                        sqlCommand.Parameters.AddWithValue($"@email{i + 1}", driver[i + 1].Email);
                        sqlCommand.Parameters.AddWithValue($"@phoneNumber{i + 1}", driver[i + 1].PhoneNumber);
                    }

                    var response = await sqlCommand.ExecuteNonQueryAsync();
                    SC.Close();
                    return response == 10;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error haappened while adding new driver with hmessage {ex.Message}");
            }
            return false;
        }

        private DriverModel PopulateDriverModel(SQLiteDataReader response) =>
              new DriverModel
              {
                  FirstName = response["FirstName"].ToString(),
                  LastName = response["LastName"].ToString(),
                  Email = response["Email"].ToString(),
                  PhoneNumber = response["PhoneNumber"].ToString()
              };

        private async Task<List<string>> PopulateDriverList(SQLiteDataReader response)
        {
            var driverList = new List<string>();
            string firstName = "", lastName = "";
            while (await response.ReadAsync())
            {
                firstName = response["FirstName"].ToString();
                lastName = response["LastName"].ToString();

                if (!char.IsUpper(firstName[0]))
                {
                    firstName = CapitalizeName(firstName);
                }
                if (!char.IsUpper(lastName[0]))
                {
                    lastName = CapitalizeName(lastName);
                }
                driverList.Add(new string(firstName + " " + lastName));
            }
            return driverList;
        }

        private string CapitalizeName(string name)
        {
            char firstChar = name[0];
            name = name.TrimStart(firstChar);
            return char.ToUpper(firstChar) + name;
        }
    }
}