namespace Tutorial3_Task;

public interface IDeviceManager
{
   
        List<Device> getAllDevices();
        Device getDeviceById(string id);
        void DeleteDevice(string id);
        void AddDevice(String jsonString);
        void UpdateDevice(string jsonString);
    
}