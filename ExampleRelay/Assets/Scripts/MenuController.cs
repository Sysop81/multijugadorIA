using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuickStart
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private NRMExt networkRoomManager;
        
        /// <summary>
        /// Method StartHost
        /// This method set a network "host" - a server and client in the same application.
        /// </summary>
        public void StartHost()
        {
            networkRoomManager.StartHost();
        }
    
        /// <summary>
        /// Method StartClient
        /// This method starts the client, connects it to the server with networkAddress.
        /// </summary>
        public void StartClient()
        {
            networkRoomManager.StartClient();
        }
    
        /// <summary>
        /// Method SetNetworkAddress
        /// This method handle the server Ip address
        /// </summary>
        /// <param name="networkAddress">Server Ip address</param>
        public void SetNetworkAddress(string networkAddress)
        {
            networkRoomManager.networkAddress = networkAddress;
        }
    
        /// <summary>
        /// Method StartServer
        /// This method starts the server mode
        /// </summary>
        public void StartServer()
        {
            networkRoomManager.StartServer();
        }
    }
}
