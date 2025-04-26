using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Tutorial3_Task;

public class DeviceManager : IDeviceManager
{
   private readonly string _connectionString;

   public DeviceManager(string connectionString)
   {
      _connectionString = connectionString;
   }

   public List<Device> getAllDevices()
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

   public Device getDeviceById(string id)
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

   public void DeleteDevice(string id)
   {
      using (SqlConnection connection = new SqlConnection(_connectionString))
      {
         connection.Open();

         string deleteSmartwatchQuery = "DELETE FROM Smartwatch WHERE DeviceId = @Id";
         SqlCommand deleteSmartwatchCommand = new SqlCommand(deleteSmartwatchQuery, connection);
         deleteSmartwatchCommand.Parameters.AddWithValue("@Id", id);
         deleteSmartwatchCommand.ExecuteNonQuery();

         string deleteDeviceQuery = "DELETE FROM Device WHERE Id = @Id";
         SqlCommand deleteDeviceCommand = new SqlCommand(deleteDeviceQuery, connection);
         deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
         deleteDeviceCommand.ExecuteNonQuery();
      }
   }

   public void AddDevice(String jsonString)
   {
      using (JsonDocument document = JsonDocument.Parse(jsonString))
      {
         JsonElement root = document.RootElement;

         string type = root.GetProperty("Id").GetString();
         Device device;

         if (type.StartsWith("E-"))
         {
            device = JsonSerializer.Deserialize<Embedded>(jsonString);
         }
         else if (type.StartsWith("P-"))
         {
            device = JsonSerializer.Deserialize<PersonalComputer>(jsonString);
         }
         else if (type.StartsWith("SW-"))
         {
            device = JsonSerializer.Deserialize<Smartwatch>(jsonString);
         }
         else
         {
            throw new ArgumentException("Unknown device type.");
         }

         using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            if (device is Smartwatch smartwatch)
            {
                string query = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.ExecuteNonQuery();

                query = "INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES (@BatteryPercentage, @DeviceId)";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
                command.Parameters.AddWithValue("@DeviceId", smartwatch.Id);
                command.ExecuteNonQuery();
            }
            else if (device is PersonalComputer pc)
            {
                string query = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.ExecuteNonQuery();

                query = "INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES (@OperationSystem, @DeviceId)";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DeviceId", pc.Id);
                command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                command.ExecuteNonQuery();
            }
            else if (device is Embedded embedded)
            {
                string query = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.ExecuteNonQuery();

                query = "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IpAddress, @NetworkName, @DeviceId)";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DeviceId", embedded.Id);
                command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
                command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
                command.ExecuteNonQuery();
            }
         }
      }
   }

   public void UpdateDevice(string jsonString)
   {
      using (JsonDocument document = JsonDocument.Parse(jsonString))
      {
         JsonElement root = document.RootElement;

         string type = root.GetProperty("Id").GetString();
         Device device;

         if (type.StartsWith("E-"))
         {
            device = JsonSerializer.Deserialize<Embedded>(jsonString);
         }
         else if (type.StartsWith("P-"))
         {
            device = JsonSerializer.Deserialize<PersonalComputer>(jsonString);
         }
         else if (type.StartsWith("SW-"))
         {
            device = JsonSerializer.Deserialize<Smartwatch>(jsonString);
         }
         else
         {
            throw new ArgumentException("Unknown device type.");
         }

         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            // Update base Device table
            string query = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", device.Id);
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
            command.ExecuteNonQuery();

            switch (device)
            {
               case Smartwatch smartwatch:
                  query =
                     "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
                  command = new SqlCommand(query, connection);
                  command.Parameters.AddWithValue("@DeviceId", smartwatch.Id);
                  command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
                  command.ExecuteNonQuery();
                  break;

               case PersonalComputer pc:
                  query =
                     "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
                  command = new SqlCommand(query, connection);
                  command.Parameters.AddWithValue("@DeviceId", pc.Id);
                  command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                  command.ExecuteNonQuery();
                  break;

               case Embedded embedded:
                  query =
                     "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
                  command = new SqlCommand(query, connection);
                  command.Parameters.AddWithValue("@DeviceId", embedded.Id);
                  command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
                  command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
                  break;
            }
         }
      }
   }
}

