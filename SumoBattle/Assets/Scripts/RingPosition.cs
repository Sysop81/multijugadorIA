using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPosition : MonoBehaviour
{
    
    private Quaternion _fixedRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        _fixedRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = _fixedRotation;
    }
}
