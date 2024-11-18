using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveForce = 3f;
    [SerializeField] private float restartGamePlayTime = 3f;
    [SerializeField] private float powerUpForce = 20f;
    [SerializeField] private GameObject[] powerUpRings;
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private NetworkVariable<bool> hasPowerUp = new NetworkVariable<bool>(); 
    
    private Rigidbody _playerRB;
    private float _powerUpTime = 7f;
    private GameObject _spawnManager;
    private string _role;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
        
        if (IsOwner)
        {
            _role = IsHost || IsServer ? "HOST" : "CLIENT";
            CheckConnectedClientsServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        
        // Manage the player role info
        UpdateRoleTextClientRpc(_role);
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
    [ServerRpc(RequireOwnership = false)]
    private void RequestPickPowerUpServerRpc(ulong clientId, ulong powerUpId)
    {
        var powerUp = NetworkManager.Singleton.SpawnManager.SpawnedObjects[powerUpId];
        if(powerUp != null) powerUp.Despawn(true);
        
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        if(player != null) player.hasPowerUp.Value = true; // TODO change to use the method 
    }
    
    /// <summary>
    /// Method RequestActivePowerUpServerRpc
    /// Setter to hasPowerUp property
    /// </summary>
    /// <param name="active">PowerUp status</param>
    [ServerRpc(RequireOwnership = false)]
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
        transform.position = Vector3.zero;
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
    /// Method UpdateRoleTextClientRpc
    /// </summary>
    /// <param name="role"></param>
    [ClientRpc]
    private void UpdateRoleTextClientRpc(string role)
    {
        roleText.text = role;
    }
    
    /// <summary>
    /// Method OnEnable [Handler]
    /// </summary>
    private void OnEnable()
    {
        //  Subscribe to OnPowerUpChanged method to listen a networkVariable changes 
        hasPowerUp.OnValueChanged += OnPowerUpChanged;
    }
    
    /// <summary>
    /// Method OnDisable [Handler]
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe to OnPowerUpChanged method (Avoid error to despawn object)
        hasPowerUp.OnValueChanged -= OnPowerUpChanged;
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

        RequestActivePowerUpServerRpc(false);
    }
    
    /// <summary>
    /// Method OnTriggerEnter [Trigger]
    /// </summary>
    /// <param name="other">GameObject detected</param>
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Death"))
        {
            Invoke("RestartGameClientRpc",restartGamePlayTime);
        }

        if (IsOwner && other.gameObject.CompareTag("PowerUp"))
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
