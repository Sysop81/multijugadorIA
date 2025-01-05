using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button sClient, sHost, sServer;
    [SerializeField] private TextMeshProUGUI gameStatusText;
    private string playerName;
    
    private enum Modes
    {
        Host,Server,Client
    }

    private void Start()
    {
        // Desactivate buttons and text
        gameStatusText.enabled = false;
        sHost.interactable = false;
        sClient.interactable = false;
        sServer.interactable = false;
        // Set handlers to input Text field
        inputName.onValueChanged.AddListener(OnManageInput);
        inputName.onEndEdit.AddListener(OnSubmitInput);
    }
    
    /// <summary>
    /// Method OnManageInput [Handler]
    /// </summary>
    /// <param name="text"></param>
    private void OnManageInput(string text)
    {
        playerName = text;
        if (text.Length > 0)
        {
            sHost.interactable = true;
            sClient.interactable = true;
            return;
        }
        sHost.interactable = false;
        sClient.interactable = false;
    }
    
    /// <summary>
    /// Method OnSubmitInput [Handler]
    /// </summary>
    /// <param name="text"></param>
    private void OnSubmitInput(string text)
    {
        playerName = text;
        if(text.Length > 0) GameObject.Find("PlayerNameContainer").SetActive(false);
    }
    
    /// <summary>
    /// Getter GetPlayerName
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return playerName;
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

