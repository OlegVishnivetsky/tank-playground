using Unity.Netcode;
using UnityEngine;

namespace TankPlayground.Core.Network
{
    public class NetworkConnector : MonoBehaviour
    {
        public void Join() => NetworkManager.Singleton.StartClient();
        
        public void Host() => NetworkManager.Singleton.StartHost();
    }
}