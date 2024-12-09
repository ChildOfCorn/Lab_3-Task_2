using System;
using System.Collections.Generic;

public interface IConnectable
{
    void Connect(Computer device);
    void Disconnect(Computer device);
    void SendData(Computer device, string data);
    void ReceiveData(string data);
}

public abstract class Computer : IConnectable
{
    public string IPAddress { get; private set; }
    public int Power { get; private set; }
    public string OperatingSystem { get; private set; }
    public List<Computer> Connections { get; private set; }

    public Computer(string ipAddress, int power, string operatingSystem)
    {
        IPAddress = ipAddress;
        Power = power;
        OperatingSystem = operatingSystem;
        Connections = new List<Computer>();
    }

    public virtual void Connect(Computer device)
    {
        if (!Connections.Contains(device))
        {
            Connections.Add(device);
            Console.WriteLine($"{IPAddress} connected to {device.IPAddress}.");
        }
    }

    public virtual void Disconnect(Computer device)
    {
        if (Connections.Contains(device))
        {
            Connections.Remove(device);
            Console.WriteLine($"{IPAddress} disconnected from {device.IPAddress}.");
        }
    }

    public virtual void SendData(Computer device, string data)
    {
        if (Connections.Contains(device))
        {
            Console.WriteLine($"Data sent from {IPAddress} to {device.IPAddress}: {data}");
            device.ReceiveData(data);
        }
        else
        {
            Console.WriteLine($"Cannot send data: {IPAddress} is not connected to {device.IPAddress}.");
        }
    }

    public virtual void ReceiveData(string data)
    {
        Console.WriteLine($"{IPAddress} received data: {data}");
    }
}

public class Server : Computer
{
    public int StorageCapacity { get; private set; }

    public Server(string ipAddress, int power, string operatingSystem, int storageCapacity)
        : base(ipAddress, power, operatingSystem)
    {
        StorageCapacity = storageCapacity;
    }

    public void HostService()
    {
        Console.WriteLine($"Server {IPAddress} is hosting a service.");
    }
}

public class Workstation : Computer
{
    public string UserName { get; private set; }

    public Workstation(string ipAddress, int power, string operatingSystem, string userName)
        : base(ipAddress, power, operatingSystem)
    {
        UserName = userName;
    }

    public void PerformTask(string task)
    {
        Console.WriteLine($"Workstation {IPAddress} performing task: {task}");
    }
}

public class Router : Computer
{
    public int MaxConnections { get; private set; }

    public Router(string ipAddress, int power, string operatingSystem, int maxConnections)
        : base(ipAddress, power, operatingSystem)
    {
        MaxConnections = maxConnections;
    }

    public override void Connect(Computer device)
    {
        if (Connections.Count < MaxConnections)
        {
            base.Connect(device);
        }
        else
        {
            Console.WriteLine($"Router {IPAddress} cannot connect to {device.IPAddress}: maximum connections reached.");
        }
    }
}

public class Network
{
    private List<Computer> devices;

    public Network()
    {
        devices = new List<Computer>();
    }

    public void AddDevice(Computer device)
    {
        devices.Add(device);
        Console.WriteLine($"Device {device.IPAddress} added to the network.");
    }

    public void RemoveDevice(Computer device)
    {
        if (devices.Contains(device))
        {
            devices.Remove(device);
            Console.WriteLine($"Device {device.IPAddress} removed from the network.");
        }
    }

    public void ShowConnections()
    {
        foreach (var device in devices)
        {
            Console.WriteLine($"{device.IPAddress} connections: {string.Join(", ", device.Connections.ConvertAll(d => d.IPAddress))}");
        }
    }
}

class Program
{
    static void Main()
    {
        var server = new Server("192.168.1.1", 500, "Linux", 2000);
        var workstation = new Workstation("192.168.1.2", 300, "Windows", "User1");
        var router = new Router("192.168.1.254", 100, "Firmware", 2);

        var network = new Network();
        network.AddDevice(server);
        network.AddDevice(workstation);
        network.AddDevice(router);

        router.Connect(server);
        router.Connect(workstation);

        server.SendData(workstation, "Hello, Workstation!");
        workstation.PerformTask("Compile Code");

        network.ShowConnections();
    }
}
