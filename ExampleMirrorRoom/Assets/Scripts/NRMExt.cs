using System.Collections;
using System.Collections.Generic;
using Mirror;
using QuickStart;
using UnityEngine;
using UnityEngine.UI;

public class NRMExt : NetworkRoomManager
{
    //private bool _showStartButton;
    [SerializeField] public GameObject[] playerPrefabs;
    private Button _btnStartGame;
    
    public override void OnRoomServerPlayersReady()
    {
        HandleStartGame(true);
    }

    public override void OnRoomServerPlayersNotReady()
    {
        HandleStartGame(false);
    }

    private void HandleStartGame(bool isInteractable)
    {
        if (!_btnStartGame)
            _btnStartGame = GameObject.Find("Start Button").GetComponent<Button>();
        
        _btnStartGame.interactable = isInteractable;
    }
    
    /*public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        
        if (NetworkServer.active && _btnStartGame == null)
        {
            _btnStartGame = FindObjectOfType<InputPanelController>().btnStart;
            //_btnStartGame.gameObject.SetActive(true);

            _btnStartGame.onClick.RemoveAllListeners(); 
            _btnStartGame.onClick.AddListener(StartGame); 
        }
    }*/

    public void StartGame()
    {
        ServerChangeScene(GameplayScene);
    }
    
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        
        MyNetworkRoomPlayer roomPlayerComponent = roomPlayer.GetComponent<MyNetworkRoomPlayer>();

        Transform startPos = GetStartPosition();
        
        var playerPref = Instantiate(playerPrefabs[roomPlayerComponent.playerPrefabIndexSelected],startPos.position,startPos.rotation);
        
        return SetPlayerInfo(playerPref,roomPlayerComponent);
    }
    
    private GameObject SetPlayerInfo(GameObject playerP,MyNetworkRoomPlayer roomPlayerComponent)
    {
        
        //Debug.Log("player name is ---> " + roomPlayerComponent.playerName);
        //Debug.Log("player color is ---> " + roomPlayerComponent._playerColor);
        playerP.GetComponent<PlayerScript>().playerName = roomPlayerComponent.playerName;
        playerP.GetComponent<PlayerScript>().playerColor = roomPlayerComponent.playerColor;
        
        return playerP;
    }
    
    
    // Task 1 --> Set default start button
    
    /*public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Utils.IsHeadless())
        {
            base.OnRoomServerPlayersReady();
        }
        else
        {
            showStartButton = true;
        }
    }
    
    public override void OnGUI()
    {
        base.OnGUI();

        if (allPlayersReady && _showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
        {
            // set to false to hide it in the game scene
            _showStartButton = false;

            ServerChangeScene(GameplayScene);
        }
    }*/
}
