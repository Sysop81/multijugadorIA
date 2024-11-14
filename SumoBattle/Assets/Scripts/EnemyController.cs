using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveForce = 1.5f;
    
    private Rigidbody _rb;
    private GameObject _player;
    private GameObject _spawnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction  = (_player.transform.position - transform.position).normalized;
        _rb.AddForce(direction * moveForce);
    }
    
    /// <summary>
    /// Method OnTriggerEnter [Trigger]
    /// </summary>
    /// <param name="other">GameObject detected</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            
            // Call LaunchNewEnemy method from _spawnManager GameObject
            if(GameObject.FindObjectsOfType<EnemyController>().Length - 1 == 0)
                _spawnManager.GetComponent<SpawnManager>().LaunchNewEnemyWave();
        }
    }
}
