using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinarySerializer
{
    public static void Add(object obj)
    {
        switch (obj)
        {
            case int i:
                {
                    _byteList.Add((byte)BinaryType.Integer);
                    _byteList.AddRange(SerializeInt(i));

                    break;
                }
            case float f:
                {
                    _byteList.Add((byte)BinaryType.Float);
                    _byteList.AddRange(SerializeFloat(f));

                    break;
                }
            case string s:
                {
                    _byteList.Add((byte)BinaryType.String);
                    _byteList.AddRange(SerializeString(s));

                    break;
                }
        }
    }

    public static byte[] Get()
    {
        _byteList.InsertRange(0, SerializeInt(_byteList.Count));

        return _byteList.ToArray();
    }

    public static void Clear()
    {
        _byteList.Clear();
    }

    private static List<byte> SerializeInt(int num)
    {
        List<byte> bytes = new List<byte>();

        for (int i = 3; i >= 0; i--)
            bytes.Add((byte)(((num >> (i * 8))) & 0xFF));

        return bytes;
    }

    private static unsafe List<byte> SerializeFloat(float num)
    {
        int val = *((int*)&num);

        return SerializeInt(val);
    }

    private static List<byte> SerializeString(string text)
    {
        List<byte> bytes = new List<byte>();

        bytes.AddRange(SerializeInt(text.Length));

        for (int i = 0; i < text.Length; i++)
            bytes.Add((byte)text[i]);

        return bytes;
    }

    private static List<byte> _byteList = new List<byte>();
}

public enum BinaryType
{
    Integer,
    Float,
    String,
    

}
