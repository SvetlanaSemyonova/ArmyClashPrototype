using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts
{
    public class EntryController : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Root");
        }
    }
}
