using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
namespace Tutorial3_Task.Tests;

[TestClass]
public class DeviceManagerTest
{
// making sure that after adding a device, it's correctly listed in DevicEManager
    [TestMethod]
    public void METHOD()
    {
        // arrange
        IDeviceManager deviceManager = DeviceManagerFactory.CreateDeviceManager();
        IDevice printer = new Printer();

        // act
        deviceManager.AddDevice(printer);

        //
        // assert
        using (StringWriter sw = new StringWriter())
        {
            Console.SetOut(sw);
            deviceManager.ListDevices();
            string output = sw.ToString();
            Assert.Contains("- Printer", output);

        }
    }
    [TestMethod]// confirming that removing a device removes it from the list
    public void RemoveDevice_Should_RemoveDeviceFromList()
    {
        // arrange
        IDeviceManager deviceManager = DeviceManagerFactory.CreateDeviceManager();
        IDevice printer = new Printer();
        deviceManager.AddDevice(printer);
        
        // act
        deviceManager.RemoveDevice(printer);
        
        // assert
        using (StringWriter sw = new StringWriter())
        {
            Console.SetOut(sw);
            deviceManager.ListDevices();
            string output = sw.ToString();
            Assert.DoesNotContain("- Printer", output);
        }
    }
}  
