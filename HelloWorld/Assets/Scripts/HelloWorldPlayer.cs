using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HelloWorldPlayer : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
    
    // Update is called once per frame
    void Update()
    {
        transform.position = position.Value;
    }
    
    /// <summary>
    /// Method OnNetworkSpawn [Override method]
    /// This method check if player is an owner to execute the move method
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }
    
    /// <summary>
    /// Method Move
    /// This method move the player based on whether if is a server or client
    /// </summary>
    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // If is server
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            position.Value = randomPosition;
        }
        else
        {
            // If is a client, need a RPC call to request a new position
            SubmitPositionRequestServerRpc();
        }
    }
    
    /// <summary>
    /// Method SubmitPositionRequestServerRpc
    /// This method is calling from client to execute in server. The client need update a transform position
    /// </summary>
    /// <param name="rpcParams"></param>
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }
    
    /// <summary>
    /// Method GetRandomPositionOnPlane
    /// This method return a vector3 with a new transform position
    /// </summary>
    /// <returns></returns>
    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }
    
}
