namespace Tutorial3_Task.Repository;

public interface IDeviceRepository
{
 
    void AddDevice(Device device);
    List<Device> GetAllDevices();
    Device GetDeviceById(string id);
    void DeleteDevice(string id);

}