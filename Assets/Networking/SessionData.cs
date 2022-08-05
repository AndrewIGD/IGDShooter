public class SessionData
{
    public string roomName;
    public int playerCount;
    public int maxPlayerCount;

    public SessionData(string roomName, int playerCount, int maxPlayerCount)
    {
        this.roomName = roomName;
        this.playerCount = playerCount;
        this.maxPlayerCount = maxPlayerCount;
    }
}

