using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    
    
    /// <summary>
    /// Method Update
    /// </summary>
    void Update()
    {
        if(target != null) agent.SetDestination(target.position);
    }
}
