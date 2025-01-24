using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class Menu : MonoBehaviour
    {
        /// <summary>
        /// Method LoadScene
        /// Handle the canvas button to switch to the game list scene
        /// </summary>
        public void LoadScene()
        {
            SceneManager.LoadScene("MirrorSceneGameList");
        }
    }
}
