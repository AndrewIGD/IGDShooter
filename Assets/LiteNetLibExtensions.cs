using LiteNetLib;

public static class LiteNetLibExtensions
{
    public static string GetPeerID(this NetPeer peer) => peer.RemoteId.ToString();
}