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
    
    /// <summary>
    /// Method OnRoomServerPlayersReady
    /// </summary>
    public override void OnRoomServerPlayersReady()
    {
        HandleStartGame(true);
    }
    
    /// <summary>
    /// Method OnRoomServerPlayersNotReady
    /// </summary>
    public override void OnRoomServerPlayersNotReady()
    {
        HandleStartGame(false);
    }
    
    /// <summary>
    /// Method HandleStartGame
    /// This method handle the start buttom "interactable or not"
    /// </summary>
    /// <param name="isInteractable"></param>
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
    
    /// <summary>
    /// Method StartGame
    /// This method load the gameplay scene
    /// </summary>
    public void StartGame()
    {
        ServerChangeScene(GameplayScene);
    }
    
    /// <summary>
    /// Method OnRoomServerCreateGamePlayer
    /// This method build the player with the options selected in the game room
    /// </summary>
    /// <param name="conn">Client connection</param>
    /// <param name="roomPlayer">Room player based on the connection</param>
    /// <returns></returns>
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        
        MyNetworkRoomPlayer roomPlayerComponent = roomPlayer.GetComponent<MyNetworkRoomPlayer>();

        Transform startPos = GetStartPosition();
        
        var playerPref = Instantiate(playerPrefabs[roomPlayerComponent.playerPrefabIndexSelected],startPos.position,startPos.rotation);
        
        return SetPlayerInfo(playerPref,roomPlayerComponent);
    }
    
    /// <summary>
    /// Method SetPlayerInfo
    /// This method update the player with the options selected
    /// </summary>
    /// <param name="playerP">Player prefab to be spawn</param>
    /// <param name="roomPlayerComponent">Configuration of the player</param>
    /// <returns></returns>
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
