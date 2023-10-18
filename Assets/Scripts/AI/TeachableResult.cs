using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class TeachableResult : MonoBehaviour
{
    public string port = "8080";
    private WebSocket ws;

    private void Start()
    {
        ResetWebSocket();
    }

    private void Update()
    {
        if (ws == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetWebSocket();
        }
    }

    private void ResetWebSocket()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }

        ws = new WebSocket("ws://localhost:" + port);
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from " + ((WebSocket)sender).Url + ", Data : " + e.Data);
        };
    }
}
