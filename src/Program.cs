using NetworkReset;
using System.Management;

var networkCardName = "Ethernet";

using NetworkConnectivityWatcher ncw = new();

var online = await Notification.Notify("Network Reset Watcher", "Conesoft Network Reset Watcher started", "https://conesoft.net");
var resetting = !online;

if (resetting)
{
    ResetNetworkCommand(networkCardName);
}

ncw.OnConnectivityChanged += async connectivity =>
{
    if (connectivity == Connectivity.Disconnected)
    {
        Console.WriteLine("The machine is disconnected from Network");
    }
    if ((connectivity & Connectivity.Internet) != 0)
    {
        if(resetting == true)
        {
            resetting = false;
        }
        Console.WriteLine("The machine is back online");
        await Notification.Notify("Network Reset Watcher", $"The machine reconnected to the Internet on '{ncw.GetOnlineNetwork()}'", "https://conesoft.net");
    }
    if ((connectivity & Connectivity.Internet) == 0)
    {
        if (resetting == false)
        {
            resetting = true;
            ResetNetworkCommand(networkCardName);
        }
        Console.WriteLine("The machine is not connected to Internet yet");
    }
};

ncw.StartListeningToChanges();

await new TaskCompletionSource<object>().Task;

void ResetNetworkCommand(string networkCardName)
{
    try
    {
        var query = new SelectQuery($"SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID LIKE '{networkCardName}'");
        var searcher = new ManagementObjectSearcher(query);
        var adapter = searcher.Get().Cast<ManagementObject>().First();

        adapter.InvokeMethod("Disable", null);
        adapter.InvokeMethod("Enable", null);
    }
    finally
    {
    }
}