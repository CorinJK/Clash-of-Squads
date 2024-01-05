using UnityEngine;
using DevelopersHub.RealtimeNetworking.Client;

namespace Scripts
{
    public class Player : MonoBehaviour
    {
        public enum RequestsID
        {
            AUTH = 1
        }
        
        private void Start()
        {
            RealtimeNetworking.OnLongReceived += ReceivedLong;
            ConnectToServer();
        }

        private void ReceivedLong(int id, long value)
        {
            switch (id)
            {
                case 1:
                    Debug.Log(value);
                    break;
            }
        }
        
        private void ConnectionResponse(bool successful)
        {
            if (successful)
            {
                RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
                string device = SystemInfo.deviceUniqueIdentifier;
                Sender.TCP_Send((int)RequestsID.AUTH, device);
            }
            else
            {
                // Failed message with retry button
            }
            RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
        }

        public void ConnectToServer()
        {
            RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
            RealtimeNetworking.Connect();
        }

        private void DisconnectedFromServer()
        {
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            // Failed message with retry button
        }
    }
}