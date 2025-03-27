using System;
using System.Collections.Generic;

/// inrfc for managing devices.

public interface IDeviceManager
{
    /// adding a device to the manager.
    /// <param name="device">Device to be added.</param>
    void AddDevice(IDevice device);

    /// <summary>


    /// <param name="device">Device to be removed.</param>
    void RemoveDevice(IDevice device);
    
    /// Lists all managed devices.
    void ListDevices();
}


/// interface representing a generic device.
public interface IDevice
{
    string Name { get; }
    void Start();
    
    void Stop();
}


/// managing a collection of devices.

public class DeviceManager : IDeviceManager
{
    private readonly List<IDevice> _devices = new List<IDevice>();
    

    public void AddDevice(IDevice device)
    {
        _devices.Add(device);
        Console.WriteLine($"Device {device.Name} added.");
    }
 
    public void RemoveDevice(IDevice device)
    {
        _devices.Remove(device);
        Console.WriteLine($"Device {device.Name} removed.");
    }
    
    public void ListDevices()
    {
        Console.WriteLine("Devices:");
        foreach (var device in _devices)
        {
            Console.WriteLine($"- {device.Name}");
        }
    }
}


/// factorydevman class for creating instances of DeviceManager.

public static class DeviceManagerFactory
{

    ///  a new instance of DeviceManager.

    /// <returns>A new IDeviceManager instance.</returns>
    public static IDeviceManager CreateDeviceManager()
    {
        return new DeviceManager();
    }
}

public class Printer : IDevice
{
    /// <inheritdoc />
    public string Name => "Printer";

    /// <inheritdoc />
    public void Start() => Console.WriteLine("Printer started.");

    /// <inheritdoc />
    public void Stop() => Console.WriteLine("Printer stopped.");
}



class Program
{
    static void Main()
    {
        IDeviceManager deviceManager = DeviceManagerFactory.CreateDeviceManager();
        
        IDevice printer = new Printer();
        deviceManager.AddDevice(printer);
        deviceManager.ListDevices();
        deviceManager.RemoveDevice(printer);
    }
}
