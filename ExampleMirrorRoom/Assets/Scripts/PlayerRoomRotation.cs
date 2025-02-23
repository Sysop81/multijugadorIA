using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1;
    private void Update()
    {
        transform.Rotate(Vector3.up,rotationSpeed);
    }
}
