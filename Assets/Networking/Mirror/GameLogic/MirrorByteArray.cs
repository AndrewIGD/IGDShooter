using Mirror;

public struct MirrorByteArray : NetworkMessage
{
    public byte[] data;

    public MirrorByteArray(byte[] data)
    {
        this.data = data;
    }
}