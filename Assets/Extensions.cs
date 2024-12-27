using UnityEngine;

public static class Extensions
{
    public static void Print(this byte[] bytes, string message)
    {
        var str = "";
        for (int i = 0; i < bytes.Length; i++)
            str += bytes[i] + " ";
        
        Debug.Log(message + bytes.Length + ": " + str);
    }
}
