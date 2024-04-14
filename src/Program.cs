using NetworkReset;
using System.Management;

var networkCardName = "Ethernet";

using NetworkConnectivityWatcher ncw = new();

var network = ncw.GetOnlineNetwork();
await Notification.Notify("Network Reset", $"PC is {(network != null ? $"online, connected to '{network}'" : "not connected")}", "https://conesoft.net");

ncw.OnConnectivityChanged += async connectivity =>
{
    if (connectivity == Connectivity.Disconnected)
    {
        Console.WriteLine("The machine is disconnected from Network");
    }
    if ((connectivity & Connectivity.Internet) != 0)
    {
        await Notification.Notify("Network Reset", $"The machine is connected to Internet on {ncw.GetOnlineNetwork()}", "https://conesoft.net");
    }
    if ((connectivity & Connectivity.Internet) == 0)
    {
        ResetNetworkCommand(networkCardName);
        Console.WriteLine("The machine is not connected to Internet yet");
    }
};

ncw.StartListeningToChanges();

await new TaskCompletionSource<object>().Task;

static void ResetNetworkCommand(string networkCardName)
{
    try
    {
        var query = new SelectQuery($"SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID LIKE '{networkCardName}'");
        var searcher = new ManagementObjectSearcher(query);
        var adapter = searcher.Get().Cast<ManagementObject>().First();

        adapter.InvokeMethod("Disable", null);
        adapter.InvokeMethod("Enable", null);
    }
    catch (Exception)
    {
    }
}