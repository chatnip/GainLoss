using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData<T> where T : Object
{
    public static Dictionary<string, T> Data = new Dictionary<string, T>();
    public static Dictionary<string, T[]> Datas = new Dictionary<string, T[]>();

    public static T GetData(string path)
    {
        if (Data.ContainsKey(path))
        {
            return Data[path];
        }
        else
        {
            Data.Add(path, Resources.Load<T>(path));
            return Data[path];
        }
    }

    public static T[] GetDatas(string path)
    {
        if (Datas.ContainsKey(path))
        {
            return Datas[path];
        }
        else
        {
            Datas.Add(path, Resources.LoadAll<T>(path));
            return Datas[path];
        }
    }

}