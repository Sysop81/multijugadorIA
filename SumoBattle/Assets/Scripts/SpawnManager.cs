using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : NetworkBehaviour 
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private NetworkObject[] powerUps;
    private float _spawnRange = 9f;
    private float _spawnPosX, _spawnPosZ;
    
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
            GenerateRandomsPowerUps();
    }
    

    /// <summary>
    /// Generate an enemy aleatory position of spawnRange
    /// </summary>
    /// <returns></returns>
    private Vector3 GenerateSpawnPosition()
    {
        _spawnPosX = Random.Range(-_spawnRange, _spawnRange);
        _spawnPosZ = Random.Range(-_spawnRange, _spawnRange);
        
        return new Vector3(_spawnPosX, 0, _spawnPosZ);
    }
    

    /// <summary>
    /// Method GenerateRandomsPowerUps
    /// This method launch a randoms power ups from the array 
    /// </summary>
    private void GenerateRandomsPowerUps()
    {
        // Generate a num of power items for the spwan wave
        var numOfPowerUps = Random.Range(1, 4);
        // Instanciate a random power up item from array for current iteration loop.
        for (int i = 0; i < numOfPowerUps; i++)
        {
            var index = Random.Range(0, powerUps.Length);
            NetworkObject pUp = Instantiate(powerUps[index], GenerateSpawnPosition(),powerUps[index].transform.rotation);
            pUp.Spawn();
        }
    }
    
    /// <summary>
    /// Method LaunchNewEnemyWave
    /// Increment index of enemy wave and call a private method SpawnEnemyWave
    /// </summary>
    public void LaunchNewPowerUpsWave()
    {
        GenerateRandomsPowerUps();
    }
}
