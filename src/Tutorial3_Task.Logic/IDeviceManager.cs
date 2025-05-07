namespace Tutorial3_Task;

public interface IDeviceManager
{
   
        List<Device> GetAllDevices();
        Device GetDeviceById(string id);
        void DeleteDevice(string id);
        void AddDevice(String jsonString);
        void UpdateDevice(string jsonString);
    
}