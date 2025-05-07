using System.Data;
using Microsoft.Data.SqlClient;

namespace Tutorial3_Task.Repository;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public List<Device> GetAllDevices()
    {
        List<Device> devices = new List<Device>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Device";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Device device = new Device
                {
                    Id = reader["Id"].ToString(),
                    Name = reader["Name"].ToString(),
                    IsEnabled = Convert.ToBoolean(reader["IsEnabled"])
                };
                devices.Add(device);
            }
        }

        return devices;
    }
    public void AddDevice(Device device)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var command = new SqlCommand("AddDevice", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    command.Parameters.AddWithValue("@Id", device.Id);
                    command.Parameters.AddWithValue("@Name", device.Name);
                    command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                    
                    if (device is Smartwatch smartwatch)
                    {
                        command.Parameters.AddWithValue("@DeviceType", "Smartwatch");
                        command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
                    }
                    else if (device is PersonalComputer pc)
                    {
                        command.Parameters.AddWithValue("@DeviceType", "PersonalComputer");
                        command.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem);
                    }
                    else if (device is Embedded embedded)
                    {
                        command.Parameters.AddWithValue("@DeviceType", "Embedded");
                        command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
                        command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
                    }
                    
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
    public void DeleteDevice(string id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("DeleteDevice", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }
    public Device GetDeviceById(string id)
{
    Device device = null;
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();

        // Determine the type of device based on the Id prefix
        if (id.StartsWith("E-"))
        {
            string query = "SELECT d.Id, d.Name, d.IsEnabled, e.IpAddress, e.NetworkName " +
                           "FROM Device d " +
                           "JOIN Embedded e ON d.Id = e.DeviceId " +
                           "WHERE d.Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                device = new Embedded
                {
                    Id = reader["Id"].ToString(),
                    Name = reader["Name"].ToString(),
                    IsEnabled = Convert.ToBoolean(reader["IsEnabled"]),
                    IpAddress = reader["IpAddress"].ToString(),
                    NetworkName = reader["NetworkName"].ToString()
                };
            }
        }
        else if (id.StartsWith("P-"))
        {
            string query = "SELECT d.Id, d.Name, d.IsEnabled, p.OperationSystem " +
                           "FROM Device d " +
                           "JOIN PersonalComputer p ON d.Id = p.DeviceId " +
                           "WHERE d.Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                device = new PersonalComputer
                {
                    Id = reader["Id"].ToString(),
                    Name = reader["Name"].ToString(),
                    IsEnabled = Convert.ToBoolean(reader["IsEnabled"]),
                    OperatingSystem = reader["OperationSystem"].ToString()
                };
            }
        }
        else if (id.StartsWith("SW-"))
        {
            string query = "SELECT d.Id, d.Name, d.IsEnabled, s.BatteryPercentage " +
                           "FROM Device d " +
                           "JOIN Smartwatch s ON d.Id = s.DeviceId " +
                           "WHERE d.Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                device = new Smartwatch
                {
                    Id = reader["Id"].ToString(),
                    Name = reader["Name"].ToString(),
                    IsEnabled = Convert.ToBoolean(reader["IsEnabled"]),
                    BatteryLevel = Convert.ToInt16(reader["BatteryPercentage"])
                };
            }
        }
    }

    return device;
}
}