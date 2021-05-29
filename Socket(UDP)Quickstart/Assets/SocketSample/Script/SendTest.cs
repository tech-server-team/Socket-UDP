using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTest : MonoBehaviour
{
    void Start()
    {
        JsonOBJ obj = new JsonOBJ(eventType.Message);
        obj.msg = "Hello World";
        NetworkManager.instance.Emit(obj);
    }
}
