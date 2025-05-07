using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Tutorial3_Task.Repository;

namespace Tutorial3_Task;

public class DeviceManager : IDeviceManager
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceManager(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
    }

    public List<Device> GetAllDevices()
    {
       return _deviceRepository.GetAllDevices();
    }

    public Device GetDeviceById(string id)
    {
        return _deviceRepository.GetDeviceById(id);
    }
    
    public void DeleteDevice(string id)
    {
       _deviceRepository.DeleteDevice(id);
    }

   public void AddDevice(String jsonString)
   {
       Device device = JsonSerializer.Deserialize<Device>(jsonString);
   
       if (device.Id.StartsWith("E-"))
       {
           device = JsonSerializer.Deserialize<Embedded>(jsonString);
       }
       else if (device.Id.StartsWith("P-"))
       {
           device = JsonSerializer.Deserialize<PersonalComputer>(jsonString);
       }
       else if (device.Id.StartsWith("SW-"))
       {
           device = JsonSerializer.Deserialize<Smartwatch>(jsonString);
       }
       else
       {
           throw new ArgumentException("Unknown device type.");
       }
   
       _deviceRepository.AddDevice(device);
   }

   public void UpdateDevice(string jsonString)
   {
       throw new NotImplementedException();
   }

   // public void UpdateDevice(string jsonString)
   // {
   //    using (JsonDocument document = JsonDocument.Parse(jsonString))
   //    {
   //       JsonElement root = document.RootElement;
   //
   //       string type = root.GetProperty("Id").GetString();
   //       Device device;
   //
   //       if (type.StartsWith("E-"))
   //       {
   //          device = JsonSerializer.Deserialize<Embedded>(jsonString);
   //       }
   //       else if (type.StartsWith("P-"))
   //       {
   //          device = JsonSerializer.Deserialize<PersonalComputer>(jsonString);
   //       }
   //       else if (type.StartsWith("SW-"))
   //       {
   //          device = JsonSerializer.Deserialize<Smartwatch>(jsonString);
   //       }
   //       else
   //       {
   //          throw new ArgumentException("Unknown device type.");
   //       }
   //
   //       using (SqlConnection connection = new SqlConnection(_connectionString))
   //       {
   //          connection.Open();
   //
   //          // Update base Device table
   //          string query = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
   //          SqlCommand command = new SqlCommand(query, connection);
   //          command.Parameters.AddWithValue("@Id", device.Id);
   //          command.Parameters.AddWithValue("@Name", device.Name);
   //          command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
   //          command.ExecuteNonQuery();
   //
   //          switch (device)
   //          {
   //             case Smartwatch smartwatch:
   //                query =
   //                   "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
   //                command = new SqlCommand(query, connection);
   //                command.Parameters.AddWithValue("@DeviceId", smartwatch.Id);
   //                command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
   //                command.ExecuteNonQuery();
   //                break;
   //
   //             case PersonalComputer pc:
   //                query =
   //                   "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
   //                command = new SqlCommand(query, connection);
   //                command.Parameters.AddWithValue("@DeviceId", pc.Id);
   //                command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
   //                command.ExecuteNonQuery();
   //                break;
   //
   //             case Embedded embedded:
   //                query =
   //                   "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
   //                command = new SqlCommand(query, connection);
   //                command.Parameters.AddWithValue("@DeviceId", embedded.Id);
   //                command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
   //                command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
   //                break;
   //          }
   //       }
   //    }
   // }
}

