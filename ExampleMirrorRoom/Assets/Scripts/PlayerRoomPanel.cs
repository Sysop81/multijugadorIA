using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerRoomPanel : NetworkBehaviour
{
    [Header("Player Panel Fields")]
    [SerializeField] [SyncVar(hook = nameof(SetPlayerName))] public string playerName;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] [SyncVar(hook = nameof(SetPlayerReady))] public bool playerReady;
    [SerializeField] private TextMeshProUGUI playerReadyTxt;
    
    
    /// <summary>
    /// Method Start
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        transform.SetParent(GameObject.Find("Players Zone").transform);
        transform.localScale = new Vector3(1,1,1);
        
        DrawPlayerNameText();
        DrawReadyText();
    }
    
    /// <summary>
    /// Method SetPlayerName [Hook]
    /// </summary>
    /// <param name="oldName"></param>
    /// <param name="newName"></param>
    void SetPlayerName(string oldName,string newName)
    {
        playerName = newName;
        DrawPlayerNameText();
    }
    
    /// <summary>
    /// Method DrawPlayerNameText
    /// </summary>
    void DrawPlayerNameText()
    {
        playerNameTxt.text = playerName;
    }
    
    /// <summary>
    /// Method SetPlayerReady [Hook]
    /// </summary>
    /// <param name="oldReady"></param>
    /// <param name="newReady"></param>
    void SetPlayerReady(bool oldReady, bool newReady)
    {
        playerReady = newReady;
        DrawReadyText();
    }
    
    /// <summary>
    /// Method DrawReadyText
    /// </summary>
    void DrawReadyText()
    {
        playerReadyTxt.text = playerReady ? "READY" : "NOT READY";
    }
}
