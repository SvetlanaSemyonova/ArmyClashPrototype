using UnityEngine;

namespace Content.Scripts.UI
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] private GameObject endGamePanel;
        [SerializeField] private GameObject win;
        [SerializeField] private GameObject defeat;

        public void OpenEndPanel(bool state)
        {
            endGamePanel.SetActive(true);
            
            win.SetActive(state);
            defeat.SetActive(!state);
        }
    }
}
