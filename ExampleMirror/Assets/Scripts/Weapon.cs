using UnityEngine;
 
 public class Weapon : MonoBehaviour
 {        
          /// <summary>
          /// weaponSpeed. Setting the speed parameter multiplier to bullet speed
          /// </summary>
          public float weaponSpeed = 15.0f;
          /// <summary>
          /// weaponLife. Bullet life when is shot
          /// </summary>
          public float weaponLife = 3.0f;
          /// <summary>
          /// weaponCooldown. Waiting time to use the weapon again
          /// </summary>
          public float weaponCooldown = 1.0f;
          /// <summary>
          /// weaponAmmo. Available bullets
          /// </summary>
          public int weaponAmmo = 15;
          /// <summary>
          /// weaponBullet. Bullet prefab
          /// </summary>
          public GameObject weaponBullet;
          /// <summary>
          /// weaponFirePosition. Transform to the weapon fire position
          /// </summary>
          public Transform weaponFirePosition;
}
