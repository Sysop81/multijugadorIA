using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] powerUps;
    private int _enemyWave = 1;
    private float spawnRange = 9f;
    private float spawnPosX, spawnPosZ;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(_enemyWave);
    }
    

    /// <summary>
    /// Generate an enemy aleatory position of spawnRange
    /// </summary>
    /// <returns></returns>
    private Vector3 GenerateSpawnPosition()
    {
        spawnPosX = Random.Range(-spawnRange, spawnRange);
        spawnPosZ = Random.Range(-spawnRange, spawnRange);
        
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
    
    /// <summary>
    /// Method SpawnEnemyWave
    /// This method generate a new enemy wave and power ups
    /// </summary>
    /// <param name="numOfEnemies">Number of enemies to instantiate</param>
    private void SpawnEnemyWave(int numOfEnemies)
    {
        // Instanciate a new random enemy from array for each loop iteration
        for (int i = 0; i < numOfEnemies; i++)
        {
            var index = Random.Range(0, enemies.Length);
            Instantiate(enemies[index], GenerateSpawnPosition(), enemies[index].transform.rotation);
        }
        // Finally call the method to build the powerUps
        GenerateRandomsPowerUps();
    }

    /// <summary>
    /// Method GenerateRandomsPowerUps
    /// This method launch a randoms power ups from the array 
    /// </summary>
    private void GenerateRandomsPowerUps()
    {
        // Generate a num of power items for the spwan wave
        var numOfPowerUps = Random.Range(0, 2);
        // Instanciate a random power up item from array for current iteration loop. And set the power up as son of Island prefab 
        for (int i = 0; i < numOfPowerUps; i++)
        {
            var index = Random.Range(0, powerUps.Length);
            var pUp = Instantiate(powerUps[index], GenerateSpawnPosition(),powerUps[index].transform.rotation);
            pUp.transform.SetParent(GameObject.FindGameObjectWithTag("Island").transform);
        }
    }
    
    /// <summary>
    /// Method LaunchNewEnemyWave
    /// Increment index of enemy wave and call a private method SpawnEnemyWave
    /// </summary>
    public void LaunchNewEnemyWave()
    {
        _enemyWave++;
        SpawnEnemyWave(_enemyWave);
    }
}
