using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public SceneReference sceneReference;
        public TextMeshProUGUI canvasStatusText;
        public PlayerScript playerScript;
        public TextMeshProUGUI canvasAmmoText;

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;
        public Button btnChangeScene;

        [SerializeField]private Button btnStopServer;
        
        /// <summary>
        /// Method Start [Life cycle] 
        /// </summary>
        private void Start()
        {
            // Only show the change scene btn in server
            btnChangeScene.gameObject.SetActive(isServer);
            btnStopServer.gameObject.SetActive(isServer);
        }

        /// <summary>
        /// Method OnStatusTextChanged [Hook]
        /// Subscribed method to manages the synchronized variable statusText.
        /// </summary>
        /// <param name="_Old">Old sync value</param>
        /// <param name="_New">New sync value</param>
        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }
        
        /// <summary>
        /// Method ButtonSendMessage
        /// Handle the canvas button UI
        /// </summary>
        public void ButtonSendMessage()
        {
            if (playerScript != null)  
                playerScript.CmdSendPlayerMessage();
        }
        
        /// <summary>
        /// Method ButtonChangeScene
        /// Handle the canvas button UI to change scene
        /// </summary>
        public void ButtonChangeScene()
        {
            // Only the server change the scene
            if (isServer)
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "MirrorSceneMyScene")
                    NetworkManager.singleton.ServerChangeScene("MirrorSceneMyOtherScene");
                else
                    NetworkManager.singleton.ServerChangeScene("MirrorSceneMyScene");
            }
            else
                Debug.Log("You are not Host.");
        }
        
        /// <summary>
        /// Method UIAmmo
        /// This method update and show the available ammo
        /// </summary>
        /// <param name="_value"></param>
        public void UIAmmo(int _value)
        {
            canvasAmmoText.text = "Ammo: " + _value;
        }
        
        /// <summary>
        /// Method StopServer
        /// </summary>
        public void StopServer()
        {
            NetworkManager.singleton.StopServer();
        }
        
        /// <summary>
        /// Method StopClient
        /// </summary>
        public void StopClient()
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
