using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
public class UIManager: MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debugText;
    
    private enum Modes
    {
        Host,Server,Client
    }
    
    /// <summary>
    /// Method StartHost
    /// This method launch the host mode
    /// </summary>
    public void StartHost()
    {

        debugText.text =
            GetStatusText(Modes.Host,
                NetworkManager.Singleton
                    .StartHost()); 
    }
    
    /// <summary>
    /// Method StartServer
    /// This method launch the server mode
    /// </summary>
    public void StartServer()
    {

        debugText.text =
            GetStatusText(Modes.Server,
                NetworkManager.Singleton
                    .StartServer()); 
    }
    
    /// <summary>
    /// This method launch the client mode
    /// </summary>
    public void StartClient()
    {

        debugText.text =
            GetStatusText(Modes.Client,
                NetworkManager.Singleton
                    .StartClient());
    }
    
    /// <summary>
    /// Method GetStatusText
    /// This method build the status msg
    /// </summary>
    /// <param name="mode">Mode type 'Server, Host or Client'</param>
    /// <param name="isStart">Boolean to set a msg status</param>
    /// <returns></returns>
    private string GetStatusText(Modes mode, bool isStart)
    {
        return mode + $" {(isStart ? "started" : "failed to Start")}";
    }
}

