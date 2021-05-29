# ネットワークClass解説

## Classリスト
1. NetworkManager.cs) **ネットワーク Main Class**
2. JsonOBJ.cs) **データ送受信用 Class**
3. SendTest.cs) **メッセージ送信テスト Class**
4. information.cs) **自分のクライアント情報を保存する Class**


## ネットワーク関連変数

```cs
//NetworkManager.cs
//===========================

//server IP指定
[SerializeField]
private string Addrass = "localhost";

//server Port指定
[SerializeField]
private int Port = 6060;

//今、接続しているPlayer(クライアント)
private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    
//クライアントが接続したら生成するGameObject指定
[SerializeField]
private GameObject playerPrifab;

```

## eventType定義

```cs
//NetworkManager.cs
public enum eventType
{
    init,
    match,
    chat,
    Message,
    disconnect,
    client_Reload,
    //=================================
    //ここに必要な処理タイプ追加
    //customType, 
}
```
* evectHandlerに使う、処理タイプを定義

## サーバーから受けたデータを処理
```cs
//NetworkManager.cs
private void evectHandler(JsonOBJ data)
    {
        eventType state = (eventType)Enum.Parse(typeof(eventType), data.type);

        switch (state)
        {
            case eventType.init:

                uuids.Add(data.uuid);
                
                var playerObj = Instantiate(playerPrifab);
                players.Add(data.uuid, playerObj);
                if (players.Count == 1)
                    information.uuid = data.uuid;
                break;

            case eventType.match:
                break;

            case eventType.Message:
                break;

            case eventType.client_Reload:

                players.Remove(data.uuid);
                break;

            //==================================
            //ここに必要な処理を追加
            //case eventType.customType:
            //    break;
            
            default:
                break;
        }

    }
```

## データ送受信用（カスタマイズ可能）
```cs
//JsonOBJ.cs
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
    
    //=============================
    //カスタマイズ可能
    // public float pos_x;
    // public float pos_y;

    public inform ribal;
    //=============================
    
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
```

## 自分のクライアント情報を保存
```cs
//information.cs
public class information
{
    public static string server_inform { get; set; }

    public static string uuid { get; set; } // 自分のクライアントid
    public static string name { get; set; } // 任意
}
```

# サーバーへDataを送信

## SendTest.cs
```cs
//SendTest.cs
public class SendTest : MonoBehaviour
{
    void Start()
    {
        JsonOBJ obj = new JsonOBJ(eventType.Message);
        obj.msg = "Hello World";
        NetworkManager.instance.Emit(obj);
    }
}
```