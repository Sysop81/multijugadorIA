using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    [SerializeField] private float frameCounter;
    private HelloWorldPlayer _pScript;

    void Start()
    {
        _pScript = GetComponent<HelloWorldPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            float theta = Time.frameCount / frameCounter;
            // Time.frameCount, contea el nº total de frames desde el 
            //inicio del juego, empieza en 0 y aumenta en 1 por cada update.
            transform.position = new Vector3((float) Math.Cos(theta), 1f, (float) Math.Sin(theta));
            _pScript.SetPosition(transform.position);
        }
    }
}
