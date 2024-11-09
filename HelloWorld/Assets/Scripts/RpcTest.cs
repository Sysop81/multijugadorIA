using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{
    /// <summary>
    /// Method OnNetworkSpawn [Override method]
    /// </summary>
    public override void OnNetworkSpawn()
    {
        //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        if (!IsServer && IsOwner) 
        {
            TestServerRpc(0, NetworkObjectId);
        }
    }
    
    /// <summary>
    /// Method TestClientRpc
    /// This method is called on the Server and runs on all connected clients
    /// </summary>
    /// <param name="value">RCP value</param>
    /// <param name="sourceNetworkObjectId">source Object ID</param>
    [ClientRpc]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }
    
    /// <summary>
    /// Method TestServerRpc
    /// This method is called fron a client and runs in server
    /// </summary>
    /// <param name="value">RCP value</param>
    /// <param name="sourceNetworkObjectId">source Object ID</param>
    [ServerRpc]
    void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
}
