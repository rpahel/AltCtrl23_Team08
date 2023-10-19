using System;
using UnityEngine;
using WebSocketSharp;

namespace ScrollShop.AI
{
    public class AiManager : MonoBehaviour
    {
        //== Fields ======================================
        [SerializeField] private AiManagerProxy _proxy;
        [SerializeField] private Bridge _bridgeSO;
        public Bridge GetBridge => _bridgeSO;
        public string port = "8080";
        private WebSocket ws;

        //== Private Methods =============================
        private void Awake()
        {
            if(_proxy)
                _proxy.Manager = this;

            ResetWebSocket();
        }

        private void Start()
        {
            if(_bridgeSO)
                _bridgeSO.Initialize();
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

        private void OnDestroy()
        {
            if(_bridgeSO)
                _bridgeSO.Terminate();

            if(_proxy && _proxy.Manager == this)
                _proxy.Manager = null;
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
                string playerType = e.Data.Split("$")[0];
                string poseName = e.Data.Split("$")[1];
                string confidence = e.Data.Split("$")[2];

                int playerIndex = -1;
                if (playerType.Equals("Spell"))
                    playerIndex = 0;
                else if (playerType.Equals("Type"))
                    playerIndex = 1;

                _bridgeSO.SetPose(playerIndex, poseName);
            };
        }
    }
}
