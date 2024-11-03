using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            float theta = Time.frameCount / 10.0f;
            // Time.frameCount, contea el nº total de frames desde el 
            //inicio del juego, empieza en 0 y aumenta en 1 por cada update.
            transform.position = new Vector3((float) Math.Cos(theta), 0.0f, (float) Math.Sin(theta));
        }
    }
}
