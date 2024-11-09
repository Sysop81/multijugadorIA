using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// HelloWorldManager "GUI"
/// </summary>
 public class HelloWorldManager : MonoBehaviour
    {
        /// <summary>
        /// Method OnGUI
        /// This method defines the user interface for server and client
        /// </summary>
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                SubmitNewPosition();
            }
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Method StartButtons
        /// This method creates the Host,client and server buttons and sets a handled method for each of them
        /// </summary>
        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
        
        /// <summary>
        /// Method StatusLabels
        /// This method set the text on GUI labels to show information to the user
        /// </summary>
        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }
        
        /// <summary>
        /// Method SubmitNewPosition
        /// This method creates a move or send an RPC to server to update a player position button. To move the player 
        /// it is necesary to call the move method an HelloWorlPlayer script asigned to Player prefab
        /// </summary>
        static void SubmitNewPosition()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
            {
                // Only if is Server
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
                { 
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().
                    Move();
                }
                else
                {
                    // Client or host
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
