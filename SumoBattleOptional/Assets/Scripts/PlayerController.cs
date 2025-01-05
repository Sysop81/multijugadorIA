using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStatustext;
    [SerializeField] private float moveForce = 25f;
    [SerializeField] private float restartGamePlayTime = 3f;
    [SerializeField] private float powerUpForce = 20f;
    [SerializeField] private GameObject[] powerUpRings;
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private NetworkVariable<bool> hasPowerUp = new NetworkVariable<bool>(); 
    [SerializeField] private NetworkVariable<bool> isWinner = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<bool> isDeath = new NetworkVariable<bool>();
    
    private Rigidbody _playerRB;
    private float _powerUpTime = 7f;
    private GameObject _spawnManager;
    private NetworkVariable<FixedString128Bytes> _role = new NetworkVariable<FixedString128Bytes>();
    private UIManager _uiManager;
    
    /// <summary>
    /// Method Awake
    /// </summary>
    private void Awake()
    {
        _role.OnValueChanged += UpdateRoleText; // Subscribes the network property to method UpdateRoleText
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    /// <summary>
    /// Method Start
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
        gameStatustext = GameObject.Find("GameStatusText").GetComponent<TextMeshProUGUI>();
        
        if (IsOwner)
        {
            CheckConnectedClientsServerRpc();
        }
    }
    
    /// <summary>
    /// Method OnNetworkSpawn [Override]
    /// Manages the player names
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            string generatedName = _uiManager.GetPlayerName();
            SubmitPlayerNameServerRpc(generatedName);
        }
        
        roleText.text = _role.Value.ToString();
    }
    
    /// <summary>
    /// Method SubmitPlayerNameServerRpc
    /// Set the player role name
    /// </summary>
    /// <param name="playerName">Player name</param>
    [ServerRpc]
    private void SubmitPlayerNameServerRpc(string playerName)
    {
        // Set the player name to network variable
        _role.Value = playerName;
        
    }
    
    /// <summary>
    /// Method UpdateRoleText
    /// This method is the handler to listen to changes in the network variable
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="newValue"></param>
    private void UpdateRoleText(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        // Actualizar el texto del rol cuando cambie el valor
        roleText.text = newValue.ToString();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!IsOwner) return;
        
        // If is owner get the local movement input and call ServerRCP method
        RequestMoveServerRpc(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    }
    
    /// <summary>
    /// Method CheckConnectedClientsServerRpc
    /// This method call the SpawnPowerUpClientRpc if the number of connected clients is greater than 1.
    /// </summary>
    [ServerRpc]
    void CheckConnectedClientsServerRpc()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count > 1) SpawnPowerUpClientRpc();
    }

    /// <summary>
    /// Method RequestMoveServerRpc
    /// This method executes the players movements
    /// </summary>
    /// <param name="hInput">Horizontal axis input</param>
    /// <param name="vInput">Vertical axis input</param>
    [ServerRpc]
    void RequestMoveServerRpc(float hInput, float vInput)
    {
        // Only the server can update any player position
        if (IsServer)
        {
            _playerRB.AddForce(new Vector3(hInput, 0, vInput).normalized * moveForce, ForceMode.Force);
        }
    }
    
    /// <summary>
    /// Method RequestPickPowerUpServerRpc
    /// This method get player and poweUps IDs when player get a powerUp and get the network object to;
    ///  1º -> PowerUp case -> Despawn the networkObject
    ///  2º -> Player case -> Set the network player property (hasPowerUp) to true value.
    /// </summary>
    /// <param name="clientId">Player ID</param>
    /// <param name="powerUpId">PowerUp ID</param>
    [ServerRpc]
    private void RequestPickPowerUpServerRpc(ulong clientId, ulong powerUpId)
    {
        var powerUp = NetworkManager.Singleton.SpawnManager.SpawnedObjects[powerUpId];
        if(powerUp) powerUp.Despawn(true);

        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        if(player) player.hasPowerUp.Value = true;
    }
    
    /// <summary>
    /// Method RequestPlayerDeathServerRpc
    /// This method manages death client and determines who is the winner
    /// </summary>
    /// <param name="clientId">Client ID</param>
    [ServerRpc]
    private void RequestPlayerDeathServerRpc(ulong clientId)
    {
        // Current instance or get the instance by client ID. Set true the network property isDeath and move the player
        // to looser area
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        player.isDeath.Value = true;
        var looserPlattform = GameObject.Find("Loosers Space").transform.position;
        player.transform.position = new Vector3(looserPlattform.x, looserPlattform.y, looserPlattform.z);
        
        // Calaculate the player who have died and update the local counter "pLooserCounter" and what player is the winner
        PlayerController pWinner = null;
        var pWinnerCounter = 0;
        var pLooserCounter = 0;
        var clients = NetworkManager.Singleton.ConnectedClients;
        foreach (KeyValuePair<ulong, NetworkClient> client in clients)
        {
            NetworkClient networkClient = client.Value;
            var pController = networkClient.PlayerObject.GetComponent<PlayerController>();
            //Debug.Log(pController._role.Value + " is death : " + pController.isDeath.Value);
            if (!pController.isDeath.Value)
            {
                pWinner = pController;
                pWinnerCounter++;
            }
            else
            {
                pLooserCounter++;
            }
        }
        // Ccheck for winner
        if (pWinnerCounter == 1 && pWinner) pWinner.isWinner.Value = true;
        
        // Restart game
        if (NetworkManager.Singleton.ConnectedClients.Count() - pLooserCounter == 0 || pWinnerCounter == 1)
        {
            DespawnPowerUpClientRpc();
            Invoke("RestartGameClientRpc",restartGamePlayTime);
        }
    }
    
    
    /// <summary>
    /// Method RequestActivePowerUpServerRpc
    /// Setter to hasPowerUp property
    /// </summary>
    /// <param name="active">PowerUp status</param>
    [ServerRpc]
    void RequestActivePowerUpServerRpc(bool active)
    {
        hasPowerUp.Value = active;
    }
    
    /// <summary>
    /// Method RestartGame
    /// This method restart game when any player fallen to death zone 
    /// </summary>
    [ClientRpc]
    public void RestartGameClientRpc()
    {
        if(!IsServer) return;
        var clients = NetworkManager.Singleton.ConnectedClients;
        foreach (KeyValuePair<ulong, NetworkClient> client in clients)
        {
            NetworkClient networkClient = client.Value;
            var pController = networkClient.PlayerObject.GetComponent<PlayerController>();
            pController.transform.position = Vector3.zero;
            pController.isDeath.Value = false;
            pController.hasPowerUp.Value = false;
            pController.isWinner.Value = false;
        }
        
        SpawnPowerUpClientRpc();
    }
    
    
    /// <summary>
    /// Methdo SpawnPowerUpClientRpc
    /// </summary>
    [ClientRpc]
    private void SpawnPowerUpClientRpc()
    {
        if(IsServer)
            // Call the LaunchEnemyWave to load a new enemy wave from spawn manager.
            _spawnManager.GetComponent<SpawnManager>().LaunchNewPowerUpsWave();
    }
    
    /// <summary>
    /// Method DespawnPowerUpClientRpc
    /// Call to intance method af SpawnManager class to despawn the power ups network objects
    /// </summary>
    [ClientRpc]
    private void DespawnPowerUpClientRpc()
    {
        if (IsServer)
            _spawnManager.GetComponent<SpawnManager>().DespawnOldPowerUps();
    }
    

    /// <summary>
    /// Method OnEnable [Handler]
    /// </summary>
    private void OnEnable()
    {
        //  Subscribe to OnPowerUpChanged method to listen a networkVariable changes
        hasPowerUp.OnValueChanged += OnPowerUpChanged;
        isDeath.OnValueChanged += OnIsDeathChanged;
        isWinner.OnValueChanged += OnIsWinnerChanged;
    }

    /// <summary>
    /// Method OnDisable [Handler]
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe to OnPowerUpChanged method (Avoid error to despawn object)
        hasPowerUp.OnValueChanged -= OnPowerUpChanged;
        isDeath.OnValueChanged -= OnIsDeathChanged;
        isWinner.OnValueChanged -= OnIsWinnerChanged;
    }
    
    /// <summary>
    /// Method OnIsWinnerChanged
    /// This method manage the game status text based on network property isWinner
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnIsWinnerChanged(bool oldValue, bool newValue)
    {
        if(!IsOwner) return;
        
        if (newValue)
        {
            gameStatustext.text = $"WINNER {_role.Value}";
            gameStatustext.enabled = true;
            return;
        }
        gameStatustext.enabled = false;
    }
    
    /// <summary>
    /// Method OnIsDeathChanged
    /// This method manage the game status text based on network property death
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnIsDeathChanged(bool oldValue, bool newValue)
    {
        if(!IsOwner) return;
        
        if (newValue)
        {
            gameStatustext.text = "GAME OVER";
            gameStatustext.enabled = true;
            return;
        }
        gameStatustext.enabled = false;
    }

    /// <summary>
    /// Method OnPowerUpChanged
    /// This method launch the corrutine when the player gets the powerUp
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnPowerUpChanged(bool oldValue, bool newValue)
    {
        // If player get the powerUp -> Launch an animation ring corrutine
        if (newValue)
        {
            Debug.Log($"Player ID: {OwnerClientId} has activated the powerUp.");
            StartCoroutine(PowerUpCountDown());
        }
        else
        {
            Debug.Log($"Player ID: {OwnerClientId} has lost the powerUp.");
        }
    }
    
    /// <summary>
    /// Method PowerUpCountDown [Corrutine]
    /// This method manages the standar powerUp countDown
    /// </summary>
    IEnumerator PowerUpCountDown()
    {
        for (int i = 0; i < powerUpRings.Length; i++)
        {
            powerUpRings[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(_powerUpTime / powerUpRings.Length);
            powerUpRings[i].SetActive(false);
        }

        if(IsOwner) RequestActivePowerUpServerRpc(false);
    }
    
    /// <summary>
    /// Method OnTriggerEnter [Trigger]
    /// </summary>
    /// <param name="other">GameObject detected</param>
    private void OnTriggerEnter(Collider other)
    {
        
        if (IsOwner && other.gameObject.CompareTag("Death"))
        {
            RequestPlayerDeathServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        if (IsOwner && other.gameObject.CompareTag("PowerUp") && !hasPowerUp.Value)
        {
            RequestPickPowerUpServerRpc(NetworkManager.Singleton.LocalClientId,
                other.GetComponent<NetworkObject>().NetworkObjectId);
        } 
    }
    
    /// <summary>
    /// Method OnCollisionEnter [Callback]
    /// This method start when this gameObject collision with other gameObject.
    /// </summary>
    /// <param name="other">GameObject detected</param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;
            otherRB.AddForce(awayFromPlayer * (hasPowerUp.Value ? powerUpForce : 1), ForceMode.Impulse);
        }
    }
}
