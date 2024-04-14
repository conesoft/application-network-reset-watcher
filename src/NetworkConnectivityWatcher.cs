using NETWORKLIST;
using System.Runtime.InteropServices.ComTypes;

namespace NetworkReset;

// based on code from https://www.codeproject.com/Articles/34650/How-to-use-the-Windows-NLM-API-to-get-notified-of
// thanks to @ryhuang

public class NetworkConnectivityWatcher : INetworkListManagerEvents, IDisposable
{
    private int m_cookie = 0;
    private IConnectionPoint? icp;
    private readonly INetworkListManager nlm = new NetworkListManager();

    public string? GetOnlineNetwork() => nlm.GetNetworks(NLM_ENUM_NETWORK.NLM_ENUM_NETWORK_CONNECTED).Cast<INetwork>().FirstOrDefault()?.GetName() ?? null;

    public Action<Connectivity>? OnConnectivityChanged { get; set; }

    public void ConnectivityChanged(NLM_CONNECTIVITY newConnectivity) => OnConnectivityChanged?.Invoke((Connectivity)newConnectivity);

    public bool StartListeningToChanges()
    {
        IConnectionPointContainer icpc = (IConnectionPointContainer)nlm;
        //similar event subscription can be used for INetworkEvents and INetworkConnectionEvents
        Guid tempGuid = typeof(INetworkListManagerEvents).GUID;
        icpc.FindConnectionPoint(ref tempGuid, out icp);
        if (icp == null)
        {
            return false;
        }
        icp.Advise(this, out m_cookie);
        return true;
    }

    public void StopListeningToChanges()
    {
        if (icp != null)
        {
            icp.Unadvise(m_cookie);
            m_cookie = 0;
            icp = null;
        }
    }

    public void Dispose()
    {
        if (m_cookie != 0)
        {
            StopListeningToChanges();
        }
    }
}
