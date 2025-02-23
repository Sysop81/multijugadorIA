using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class GamesList : MonoBehaviour
    {
        /// <summary>
        /// Method LoadScene
        /// Handle the canvas button to switch to the menu scene
        /// </summary>
        public void LoadScene()
        {
            SceneManager.LoadScene("MirrorSceneMenu");
        }
    }
}
