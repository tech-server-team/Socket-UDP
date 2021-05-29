using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


/// <summary>
/// Event Type
/// 処理タイプ定義
/// </summary>
public enum eventType
{
    init,
    match,
    chat,
    Message,
    disconnect,
    client_Reload,
    //customType, //処理タイプ追加
}

public class NetworkManager : MonoBehaviour
{


    static private NetworkManager _instance;
    static public NetworkManager instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            else
            {
                return _instance;
            }
        }
    }
    private Queue<string> queue;

    //server IP
    [SerializeField]
    private string Addrass = "localhost";

    //server Port
    [SerializeField]
    private int Port = 6060;

    private UdpClient client;

    private List<string> uuids = new List<string>();


    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    [SerializeField]
    private GameObject playerPrifab;

    private void Awake()
    {
        //if(information.server_inform != null)
        //{
        //}

        queue = new Queue<string>();
        StartCoroutine(CheckQueue());
        if (instance != null) return;

        Connect(Addrass, Port);
    }
    
    void OnDestroy()
    {

        JsonOBJ Massafe = new JsonOBJ(eventType.disconnect);
    
        //サーバーと接続状態の場合
        if(information.uuid != null || information.uuid != "")
        {
            Massafe.uuid = information.uuid;
            Emit(Massafe);
        }

        client.Close();

        information.server_inform = null;
    }

    //=======================================================================
    //Evecnt処理関数
    //=======================================================================
    /// <summary>
    /// Evecnt処理関数
    /// </summary>
    /// <param name="data">サーバーから受けたJsonデータ</param>
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

            //case eventType.customType:
            //    break;
            
            default:
                break;
        }

    }
//=======================================================================
//=======================================================================
//=======================================================================



    /// <summary>
    /// サーバーへ接続
    /// </summary>
    /// <param name="Adddrass"></param>
    /// <param name="Port"></param>
    void Connect(string Adddrass, int Port)
    {
        client = new UdpClient();

        try
        {
            _instance = this;
            client.Connect(Adddrass, Port);

            JsonOBJ Massafe = new JsonOBJ(eventType.init);
            Emit(Massafe);

            Thread thread = new Thread(() => Data_Resiving());
            thread.Start();
        }


        catch (Exception e)
        {
            client.Close();
            client.Dispose();
            _instance = null;

            print("Exception thrown " + e.Message);
        }
    }

    /// <summary>
    /// サーバへデータを送信する関数
    /// </summary>
    /// <param name="client"></param>
    /// <param name="Massage"></param>
    public void Emit(JsonOBJ Massage)
    {
        string msg = JsonOBJ.CreateToJSON(Massage);
        byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
        client.Send(sendBytes, sendBytes.Length);
    }

    /// <summary>
    /// サーバからデータを受信
    /// </summary>
    private void Data_Resiving()
    {
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message received from the server \n " + receivedString);

            JsonOBJ data = JsonOBJ.CreateFromJSON(receivedString);
            queue.Enqueue(receivedString); //受信されたdataの臨時保存
        }
    }

    /// <summary>
    ///臨時保存先きからunityの処理する
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckQueue()
    {
        while (true)
        {
            string data;
            if (queue.Count > 0)
            {
                data = queue.Dequeue();
                Debug.Log("queue count : " + queue.Count);
            }

            else 
                data = string.Empty;

            JsonOBJ JsonObj = JsonOBJ.CreateFromJSON(data);
            if (!data.Equals(string.Empty))
                evectHandler(JsonObj);
            yield return null;
        }
    }
}
