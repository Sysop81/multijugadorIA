using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIsland : MonoBehaviour
{
    [SerializeField] private float minTime, maxTime;
    private float _rotationSpeed;
    private float _time;
    private float _iRotationX; 
    
    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = GetRandom(2f, 4f);
        _iRotationX = transform.eulerAngles.x;
        _time = GetRandom(minTime, maxTime);
        StartCoroutine(Rotate());
    }
    
    /// <summary>
    /// Method Rotate [Corrutine]
    /// This method change the island X axis rotation
    /// </summary>
    /// <returns></returns>
    IEnumerator Rotate()
    {
        // Infinite external loop 
        while (true)
        {
            // Set and manage the rotationTime with randon _time to rotate X axis 
            float rotationTime = 0f;
            while (rotationTime < _time)
            {
                // Check a Max negative angle of rotation
                if(transform.rotation.x > -10)
                    transform.Rotate(_rotationSpeed * Time.deltaTime, 0, 0);
                // Increment the rotation time to manage exit of loop
                rotationTime += Time.deltaTime;
                
                yield return null;
            }
            // The second step launch a corrutine to set the platform to initial angle state. X = 0
            yield return StartCoroutine(ResetRotation());
            // Finally update a time and rotationSpeed to launch a new rotation platform move.
            _time = GetRandom(minTime, maxTime);
            _rotationSpeed = Random.Range(0, 2) == 0 ? -GetRandom(2, 4) : GetRandom(2, 4);
            yield return new WaitForSeconds(_time);
        }
    }
    
    /// <summary>
    /// Method ResetRotation [Corrutine]
    /// This method set the platform rotation to initial value
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetRotation()
    {
        float resetDuration = _time;  
        float resetElapsedTime = 0f;

        // Normalize with lerp interpolation. Necesary with negative rotation
        float currentRotationX = transform.eulerAngles.x > 180f ? transform.eulerAngles.x - 360f : transform.eulerAngles.x;

        while (resetElapsedTime < resetDuration)
        {
            // Calculate && update the target rotation position in this elapse time. Lerp with start in currenX to initial rotation
            float newRotationX = Mathf.Lerp(currentRotationX, _iRotationX, resetElapsedTime / resetDuration);
            transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
            
            // Upgrade reset time to end while loop and yield to next loop
            resetElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finally set the island rotation with the iRotaionX to ensure that the rotation is the initial x == 0
        transform.rotation = Quaternion.Euler(_iRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
    }
    
    /// <summary>
    /// Method GetRandom
    /// This method calculate the random number between min and max
    /// </summary>
    /// <param name="min">Min value to start random value</param>
    /// <param name="max">Max value to end rendom value</param>
    /// <returns></returns>
    private static float GetRandom(float min, float max)
    {
        return Random.Range(min, max);
    }
}
