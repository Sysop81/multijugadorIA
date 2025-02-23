using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerRoomPanel : NetworkBehaviour
{
    
    [SerializeField] [SyncVar(hook = nameof(SetPlayerName))] public string playerName;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] [SyncVar(hook = nameof(SetPlayerReady))] public bool playerReady;
    [SerializeField] private TextMeshProUGUI playerReadyTxt;
    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Players Zone").transform);
        transform.localScale = new Vector3(1,1,1);
        
        DrawPlayerNameText();
        DrawReadyText();
    }

    void SetPlayerName(string oldName,string newName)
    {
        playerName = newName;
        DrawPlayerNameText();
    }
    void DrawPlayerNameText()
    {
        playerNameTxt.text = playerName;
    }

    void SetPlayerReady(bool oldReady, bool newReady)
    {
        playerReady = newReady;
        DrawReadyText();
    }
    void DrawReadyText()
    {
        if (playerReady)
            playerReadyTxt.text = "READY";
        else
            playerReadyTxt.text = "NOT READY";
    }
}
