namespace NetworkReset;

[Flags]
public enum Connectivity
{
    Disconnected = 0,
    IPv4NoTraffic = 1,
    IPv6NoTraffic = 2,
    IPv4Subnet = 0x10,
    IPv4LocalNetwork = 0x20,
    IPv4Internet = 0x40,
    IPv6Subnet = 0x100,
    IPv6LocalNetwork = 0x200,
    IPv6Internet = 0x400,

    NoTraffic = IPv4NoTraffic | IPv6NoTraffic,
    Subnet = IPv4Subnet | IPv6Subnet,
    LocalNetwork = IPv4LocalNetwork | IPv6LocalNetwork,
    Internet = IPv4Internet | IPv6Internet
}