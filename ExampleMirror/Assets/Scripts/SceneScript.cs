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

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)  
                playerScript.CmdSendPlayerMessage();
        }
        
        public void ButtonChangeScene()
        {
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
        
        public void UIAmmo(int _value)
        {
            canvasAmmoText.text = "Ammo: " + _value;
        }
    }
}
