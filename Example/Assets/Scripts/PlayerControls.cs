using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour /*MonoBehaviour*/
{
    [SerializeField] private float speed = 0.5f;

    
    // Update is called once per frame
    void Update()
    {
        
        if(!IsOwner) return;
        
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(xInput, 0, yInput).normalized;
        transform.Translate(speed * Time.deltaTime * moveDirection);
    }
}
