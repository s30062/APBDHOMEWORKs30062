using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


public abstract class Device
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsTurnedOn { get; private set; }

    public Device(int id, string name)
    {
        Id = id;
        Name = name;
        IsTurnedOn = false;
    }

    public virtual void TurnOn()
    {
        IsTurnedOn = true;
        Console.WriteLine($"{Name} is now turned ON");
    }

    public void TurnOff()
    {
        IsTurnedOn = false;
        Console.WriteLine($"{Name} is now OFF");
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Status: {(IsTurnedOn ? "ON" : "OFF")}";
    }
}

public class EmptyBatteryException : Exception
{
    public EmptyBatteryException(string message) : base(message) { }
}

public class EmptySystemException : Exception
{
    public EmptySystemException(string message) : base(message) { }
}

public class ConnectionException : Exception
{
    public ConnectionException(string message) : base(message) { }
}


public interface IPowerNotifier
{
    void NotifyLowBattery();
}

public class Smartwatch : Device, IPowerNotifier
{
    private int _batteryPercentage;
    public int BatteryPercentage 
    {
        get => _batteryPercentage;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("Battery percentage must be between 0 and 100.");
            
            _batteryPercentage = value;
            if (_batteryPercentage < 20)
                NotifyLowBattery();
        }
    }

    public Smartwatch(int id, string name, int batteryPercentage) : base(id, name)
    {
        BatteryPercentage = batteryPercentage;
    }

    public override void TurnOn()
    {
        if (BatteryPercentage < 11)
            throw new EmptyBatteryException("Battery too low to turn on.");
        
        base.TurnOn();
        BatteryPercentage -= 10;
    }

    public void NotifyLowBattery()
    {
        Console.WriteLine($"Warning: {Name} battery is low ({BatteryPercentage}%).");
    }

    public override string ToString()
    {
        return base.ToString() + $", Battery: {BatteryPercentage}%";
    }
}

public class PersonalComputer : Device
{
    public string OperatingSystem { get; private set; }
    
    public PersonalComputer(int id, string name, string os = "") : base(id, name)
    {
        OperatingSystem = os;
    }

    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new EmptySystemException("No OS installed. Cannot turn on.");
        
        base.TurnOn();
    }

    public void InstallOS(string os)
    {
        OperatingSystem = os;
        Console.WriteLine($"{Name} installed {os}.");
    }

    public override string ToString()
    {
        return base.ToString() + $", OS: {(string.IsNullOrEmpty(OperatingSystem) ? "Not Installed" : OperatingSystem)}";
    }
}

public class EmbeddedDevice : Device
{
    private static readonly Regex IpRegex = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
    public string IPAddress { get; private set; }
    public string NetworkName { get; private set; }

    public EmbeddedDevice(int id, string name, string ipAddress, string networkName) : base(id, name)
    {
        SetIPAddress(ipAddress);
        NetworkName = networkName;
    }

    public void SetIPAddress(string ipAddress)
    {
        if (!IpRegex.IsMatch(ipAddress))
            throw new ArgumentException(" invalid IP address forma");
        IPAddress = ipAddress;
    }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new ConnectionException("device can only connect to MD ltd network");
    }

    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }

    public override string ToString()
    {
        return base.ToString() + $", IP: {IPAddress}, Network: {NetworkName}";
    }
}


public class DeviceManager
{
    private List<Device> devices = new List<Device>();
    private const int MaxCapacity = 15;
    
    public DeviceManager(string filePath)
    {
        LoadDevicesFromFile(filePath);
    }
    
    private void LoadDevicesFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        foreach (var line in File.ReadLines(filePath))
        {
            var parts = line.Split(',');
            try
            {
                string type = parts[0].Split('-')[0];
                int id = int.Parse(parts[0].Split('-')[1]);
                string name = parts[1];

                switch (type)
                {
                    case "SW":
                        AddDevice(new Smartwatch(id, name, int.Parse(parts[3].Replace("%", ""))));
                        break;
                    case "P":
                        AddDevice(new PersonalComputer(id, name, parts.Length > 3 ? parts[3] : ""));
                        break;
                    case "ED":
                        AddDevice(new EmbeddedDevice(id, name, parts[2], parts[3]));
                        break;
                    default:
                        Console.WriteLine($"Invalid entry ignored: {line}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line '{line}': {ex.Message}");
            }
        }
    }
    
    public void AddDevice(Device device)
    {
        if (devices.Count >= MaxCapacity)
            throw new InvalidOperationException("Device storage is full.");
        
        devices.Add(device);
    }
    
    public void ShowAllDevices()
    {
        foreach (var device in devices)
        {
            Console.WriteLine(device);
        }
    }
}
class Program
{
    static void Main()
    {
        string filePath = "/Users/antonsidlyar/HomeWorks30062/ConsoleApp1/ConsoleApp1/input (1).txt";
        DeviceManager manager = new DeviceManager(filePath);

        Console.WriteLine("all devices which were loaded:");
        manager.ShowAllDevices();
    }
}
