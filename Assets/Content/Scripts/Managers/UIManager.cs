using Content.Scripts.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Content.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Top Panel")]
        [SerializeField] private Slider userProgress;
        [SerializeField] private Slider aiProgress;

        [Header("Bottom Panel")] 
        [SerializeField] private GameObject bottomPanel;
        
        [SerializeField] private EndGamePanel endGamePanel;


        private void Awake()
        {
            Assert.IsNotNull(userProgress);
            Assert.IsNotNull(aiProgress);
            Assert.IsNotNull(bottomPanel);
            Assert.IsNotNull(endGamePanel);
        }

        public void RefreshView(float myUnits, float enemyUnits)
        {
            userProgress.value = myUnits;
            aiProgress.value = enemyUnits;
        }

        public void OpenEndPanel(bool state)
        {
            endGamePanel.OpenEndPanel(state);
        }

        public void EnableBottomPanel(bool state)
        {
            bottomPanel.SetActive(state);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Entry");
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene("Root");
        }
    }
}