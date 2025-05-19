using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo;
    
    /// <summary>
    /// Method Start
    /// </summary>
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }
    
    /// <summary>
    /// Method Update
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                m_Agent.destination = m_HitInfo.point;
            }
        }
    }
}
