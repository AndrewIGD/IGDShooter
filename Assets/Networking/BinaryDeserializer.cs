using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinaryDeserializer
{
    private static byte[] _bytes;
    private static int _byteIndex;

    public static object[] Deserialize(byte[] bytes)
    {
        List<object> objectList = new List<object>();
        _bytes = bytes;
        _byteIndex = 0;

        while (_byteIndex < bytes.Length)
        {
            BinaryType type = (BinaryType)DeserializeByte();

            switch (type)
            {
                case BinaryType.Integer:
                    {
                        objectList.Add(DeserializeInt());

                        break;
                    }
                case BinaryType.Float:
                    {
                        objectList.Add(DeserializeFloat());

                        break;
                    }
                case BinaryType.String:
                    {
                        objectList.Add(DeserializeString());

                        break;
                    }
            }
        }

        return objectList.ToArray();
    }

    public static int GetIntFromBytes(byte[] bytes)
    {
        int num = 0;

        int byteIndex = 0;

        for (int i = 3; i >= 0; i--)
            num += bytes[byteIndex++] << (i * 8);

        return num;
    }

    private static byte DeserializeByte()
    {
        return _bytes[_byteIndex++];
    }

    private static int DeserializeInt()
    {
        int num = 0;

        for (int i = 3; i >= 0; i--)
            num += _bytes[_byteIndex++] << (i * 8);

        return num;
    }

    private static unsafe float DeserializeFloat()
    {
        int val = DeserializeInt();

        return *(float*)&val;
    }

    private static string DeserializeString()
    {
        int size = DeserializeInt();

        string text = "";

        for (int i = 0; i < size; i++)
            text += ((char)_bytes[_byteIndex++]).ToString();

        return text;
    }
}
