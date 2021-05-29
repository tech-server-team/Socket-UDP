using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class inform
{
    public string uuid;
    public string address;
    public int port;
    public string name;
}


[Serializable]
public class JsonOBJ
{
    public string type;
    public string name;
    public string uuid;
    public string msg;

    public inform ribal;

    public JsonOBJ(eventType state)
    {
        this.type = state.ToString();
    }

    public static JsonOBJ CreateFromJSON(string data)
    {
        try
        {
            return JsonUtility.FromJson<JsonOBJ>(data);
        }
        catch (Exception e)
        {
            Debug.Log("<color=green> Client: </color> JSONデータで変換できました。");
            return null;
        }
    }
    public static string CreateToJSON(JsonOBJ data)
    {
        return JsonUtility.ToJson(data);
    }
}
