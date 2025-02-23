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
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        inputPanelController = GameObject.Find("Input Panel").GetComponent<InputPanelController>();

        if (isServer)
        {
            inputPanelController.btnStart.gameObject.SetActive(true);
            inputPanelController.btnStart.interactable = ((NRMExt)NetworkManager.singleton).allPlayersReady; 
            inputPanelController.btnStart.onClick.RemoveAllListeners();
            inputPanelController.btnStart.onClick.AddListener(delegate { ((NRMExt)NetworkManager.singleton).StartGame();});
            CmdCreatePlayerPanel();
        }

        if (isLocalPlayer)
        {
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
    
    [Command]
    private void CmdApplyColor()
    {
        playerColor = colorByIndex[index];
    }
    
    
    [Command]
    private void CmdPlayerSetName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            playerName = newName; 
            if(myRoomPlayerPanel) myRoomPlayerPanel.GetComponent<PlayerRoomPanel>().playerName = playerName;    
        }
        
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateSelectedPlayer(int indexSelected)
    {
        playerPrefabIndexSelected = indexSelected;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdCreatePlayerPanel()
    {
        myRoomPlayerPanel = Instantiate(roomPlayerPanelPrefab, GameObject.Find("Players Zone").transform);
        NetworkServer.Spawn(myRoomPlayerPanel);
    }
    
    void PlayerReadyToggle()
    {
        CmdChangeReadyState(!readyToBegin);
    }

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
