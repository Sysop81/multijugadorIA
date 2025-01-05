using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera loosersCamera;
    
    private bool isLoosersCamera;
    
    /// <summary>
    /// Method Start
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        loosersCamera.gameObject.SetActive(false);
    }

    /// <summary>
    /// Method Update
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        ToggleCamera();
    }
    
    /// <summary>
    /// Method ToggleCamera
    /// This method alternates between the main camera and loosers camera
    /// </summary>
    private void ToggleCamera()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isLoosersCamera = !isLoosersCamera;
            
            mainCamera.gameObject.SetActive(isLoosersCamera);
            loosersCamera.gameObject.SetActive(!isLoosersCamera);
        }
    }
}
