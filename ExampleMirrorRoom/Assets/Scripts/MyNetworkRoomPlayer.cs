using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private InputPanelController inputPanelController;

    [Header("Players Panel List")]
    [SerializeField] private GameObject roomPlayerPanelPrefab;
    [SerializeField] private GameObject myRoomPlayerPanel;
    [SerializeField] [SyncVar] public string playerName;
    
    [Header("Player Type Selected")]
    [SerializeField] public int playerPrefabIndexSelected;
    
    [Header("Player color")]
    [SyncVar] public Color playerColor;
    [SerializeField] private Color[] colorByIndex;
    
    /// <summary>
    /// Method Start
    /// Start is called before the first frame update
    /// </summary>
    public override void Start()
    {
        base.Start();
        // Load panel for all
        inputPanelController = GameObject.Find("Input Panel").GetComponent<InputPanelController>();

        if (isServer)
        {
            // Handle the server components
            inputPanelController.btnStart.gameObject.SetActive(true);
            inputPanelController.btnStart.interactable = ((NRMExt)NetworkManager.singleton).allPlayersReady; 
            inputPanelController.btnStart.onClick.RemoveAllListeners();
            inputPanelController.btnStart.onClick.AddListener(delegate { ((NRMExt)NetworkManager.singleton).StartGame();});
            CmdCreatePlayerPanel();
        }

        if (isLocalPlayer)
        {
            // Handle the localplayer components
            inputPanelController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
            inputPanelController.inputPlayerName.onValueChanged.AddListener(delegate(string newStringInput) { CmdPlayerSetName(newStringInput); });
            inputPanelController.btnCameraLeft.onClick.AddListener(delegate { CmdUpdateSelectedPlayer(0); });
            inputPanelController.btnCameraRight.onClick.AddListener(delegate { CmdUpdateSelectedPlayer(1); });
            inputPanelController.AppyColorToInterfazePlayers(colorByIndex[index]);
            CmdApplyColor();
            CmdUpdateSelectedPlayer(0);
            CmdPlayerSetName("Player " + index);
        }
    }
    
    /// <summary>
    /// Method CmdApplyColor
    /// This method updates the player color
    /// </summary>
    [Command]
    private void CmdApplyColor()
    {
        playerColor = colorByIndex[index];
    }
    
    /// <summary>
    /// Method CmdPlayerSetName
    /// This method handle the player name
    /// </summary>
    /// <param name="newName"></param>
    [Command]
    private void CmdPlayerSetName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            playerName = newName; 
            if(myRoomPlayerPanel) myRoomPlayerPanel.GetComponent<PlayerRoomPanel>().playerName = playerName;    
        }
        
    }
    
    /// <summary>
    /// Method CmdUpdateSelectedPlayer
    /// This method handles the player index (type of player) to spawn in the game
    /// </summary>
    /// <param name="indexSelected"></param>
    [Command(requiresAuthority = false)]
    private void CmdUpdateSelectedPlayer(int indexSelected)
    {
        playerPrefabIndexSelected = indexSelected;
    }
    
    /// <summary>
    /// Method CmdCreatePlayerPanel
    /// This method Instatiate the panel prefab in the players list
    /// </summary>
    [Command(requiresAuthority = false)]
    private void CmdCreatePlayerPanel()
    {
        myRoomPlayerPanel = Instantiate(roomPlayerPanelPrefab, GameObject.Find("Players Zone").transform);
        NetworkServer.Spawn(myRoomPlayerPanel);
    }
    
    /// <summary>
    /// Method PlayerReadyToggle
    /// This method handles the ready state
    /// </summary>
    void PlayerReadyToggle()
    {
        CmdChangeReadyState(!readyToBegin);
    }
    
    /// <summary>
    /// Method ReadyStateChanged
    /// This method changes the ready state on the player panel
    /// </summary>
    /// <param name="oldReadyState"></param>
    /// <param name="newReadyState"></param>
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (isServer)
        {
            if (myRoomPlayerPanel)
            {
                myRoomPlayerPanel.GetComponent<PlayerRoomPanel>().playerReady = newReadyState;
            }
        }
    }
}
