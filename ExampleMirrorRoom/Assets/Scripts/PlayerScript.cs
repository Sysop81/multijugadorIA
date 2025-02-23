using System.Linq;
using Mirror;
using Mirror.BouncyCastle.Cms;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        
        public TextMeshPro playerNameText;
        public GameObject floatingInfo;

        private Material playerMaterialClone;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;
        
        public SceneScript sceneScript;
        
        private int selectedWeaponLocal = 1;
        public GameObject[] weaponArray;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;
        
        private Weapon activeWeapon;
        private float weaponCooldownTime;
        
        private bool isGenerated;
        
        private void OnEnable()
        {
            // Subscribe to OnSceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // Handler to execute in clients whe scene is changed
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // TODO checking this  and refact
            
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            
            // Delete cameras
            if (isLocalPlayer)
            {
               
                Camera[] allCameras = FindObjectsOfType<Camera>();
                if (allCameras.Length > 1)
                {
                    foreach (Camera cam in allCameras)
                    {
                        if (cam.gameObject.name != "PlayerMainCamera")
                        {
                            Destroy(cam.gameObject);
                        }
                    }
                }
                // Load canvas info status
                sceneScript.statusText = $"{playerName} joined.";
                sceneScript.playerScript = this;
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
            
            
        }
        
        
        /// <summary>
        /// Method Awake [Life cycle]
        /// </summary>
        void Awake()
        {
            // Load scene script on scene reference
            sceneScript = /*GameObject.Find("SceneScript").GetComponent<SceneScript>();*/GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            
            // disable all weapons
            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false);
            
            // Select inital weapon and ammo
            if (selectedWeaponLocal < weaponArray.Length && weaponArray[selectedWeaponLocal] != null)
            {
                weaponArray[selectedWeaponLocal].SetActive(true);
                activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
                //sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
        }
        
        
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Method OnWeaponChanged [Hook]
        /// Method to which the synchronized variable activeWeaponSynced is subscribed. 
        /// </summary>
        /// <param name="_Old">Old value for SyncVar</param>
        /// <param name="_New">New value for SyncVar</param>
        void OnWeaponChanged(int _Old, int _New)
        {
            // disable old weapon
            // in range and not null
            if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
                weaponArray[_Old].SetActive(false);
    
            // enable new weapon
            // in range and not null
            if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            {
                weaponArray[_New].SetActive(true);
                activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
            }
            else
            {   
                // Set active weapon to null if weapon weaponArray[_New] is equal null
                activeWeapon = null;
            }
            
            // Set UI info
            if(isLocalPlayer) sceneScript.UIAmmo(activeWeapon ? activeWeapon.weaponAmmo:0);

        }
        
        /// <summary>
        /// Method CmdChangeActiveWeapon [Server RPC]
        /// This method is executed from the client to indicate to the server the change of state in the synchronized variable
        /// activeWeaponSynced
        /// </summary>
        /// <param name="newIndex"></param>
        [Command]
        public void CmdChangeActiveWeapon(int newIndex)
        {
            activeWeaponSynced = newIndex;
        }
        
        /// <summary>
        /// Method CmdSendPlayerMessage [Server RPC]
        /// </summary>
        [Command]
        public void CmdSendPlayerMessage()
        {
            if (sceneScript)
            {
                sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
            } 
                
        }
        
        /// <summary>
        /// Method OnNameChanged [Hook]
        /// Subscribed method to manages the synchronized variable playerNameText.
        /// </summary>
        /// <param name="_Old">Old sync value</param>
        /// <param name="_New">New sync value</param>
        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }
        
        /// <summary>
        /// Method OnColorChanged [Hook]
        /// Subscribed method to manages the synchronized variable playerMaterialClone.
        /// </summary>
        /// <param name="_Old">Old sync value</param>
        /// <param name="_New">New sync value</param>
        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }
        
        /// <summary>
        /// Method OnStartLocalPlayer [Override method]
        /// This method set the initial setting for the spawned player
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            sceneScript.statusText = $"{playerName} joined.";
            sceneScript.playerScript = this;
            sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            
            
            if (!isGenerated)
            {
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.localRotation = Quaternion.identity; 
                Camera.main.transform.localPosition = new Vector3(0, 0, 0);
                Camera.main.name = "PlayerMainCamera";
                floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
                floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                //string name = "Player" + Random.Range(100, 999);
                //Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                //CmdSetupPlayer(name, color);
                isGenerated = true;
            }
        }
        

        /// <summary>
        /// Method CmdSetupPlayer [Server RCP]
        /// </summary>
        /// <param name="_name">Player name</param>
        /// <param name="_col">Player color</param>
        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;
            sceneScript.statusText = $"{playerName} joined.";
        }
        
        /// <summary>
        /// Method Update [Life cycle]
        /// </summary>
        void Update()
        {
            if (!isLocalPlayer)
            {
                // make non-local players run this
                floatingInfo.transform.LookAt(Camera.main.transform);
                return;
            }
            
            // Only for local player
            float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.Rotate(0, moveX, 0);
            transform.Translate(0, 0, moveZ);
            
            // Fire1 is mouse 1st click.  This is a fire button
            if (Input.GetButtonDown("Fire1") ) 
            {
                if (activeWeapon && Time.time > weaponCooldownTime && activeWeapon.weaponAmmo > 0)
                {
                    weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                    activeWeapon.weaponAmmo -= 1;
                    sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                    CmdShootRay();
                }
            }
            
            // Fire2 is mouse 2nd click and left alt. Change weapon button
            if (Input.GetButtonDown("Fire2")) 
            {
                selectedWeaponLocal += 1;

                if (selectedWeaponLocal > weaponArray.Length) 
                    selectedWeaponLocal = 1; 

                CmdChangeActiveWeapon(selectedWeaponLocal);
            }
        }
        
        /// <summary>
        /// Method CmdShootRay [Server RCP]
        /// This method is called by the client who fire the weapon
        /// </summary>
        [Command]
        void CmdShootRay()
        {
            // The server calling the RpcFireWeapon method
            RpcFireWeapon();
        }
        
        /// <summary>
        /// Method RpcFireWeapon [Client RCP]
        /// All client execute this method
        /// </summary>
        [ClientRpc]
        void RpcFireWeapon()
        {
            //bulletAudio.Play(); muzzleflash  etc
            GameObject bullet = Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * activeWeapon.weaponSpeed;
            Destroy(bullet, activeWeapon.weaponLife);
        }
    }
}
