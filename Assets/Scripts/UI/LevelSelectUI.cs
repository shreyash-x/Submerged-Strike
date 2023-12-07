using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public struct LevelData
    {
        public Button levelButton;
        public TextMeshProUGUI levelText;
        public GameObject lockIcon;
    }
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private DataManager dataManager;
        [SerializeField] private LevelData[] levels;
        [SerializeField] private Button easterEggLevel;

        private int numberOfLevels;
        
        private void Start()
        {
            numberOfLevels = levels.Length;

            // Loop through each level except first
            for (var i = 1; i <= numberOfLevels; i++)
            {
                var levelButton = levels[i - 1].levelButton;
                var levelText = levels[i - 1].levelText;
                var levelLockIcon = levels[i - 1].lockIcon;
                
                // Set the level text to the level number
                levelText.text = (i).ToString();
                
                if (i == 1 || dataManager.HasCompletedLevel(i-1))
                {
                    levelButton.interactable = true;
                    levelLockIcon.SetActive(false);
                    levelText.gameObject.SetActive(true);
                }
                else
                {
                    levelButton.interactable = false;
                    levelLockIcon.SetActive(true);
                    levelText.gameObject.SetActive(false);
                }
            }

            // Easter egg level
            if (dataManager.HasCompletedLevel(numberOfLevels))
            {
                easterEggLevel.gameObject.SetActive(true);
                easterEggLevel.interactable = true;
            }
            else
            {
                easterEggLevel.gameObject.SetActive(false);
            }
        }
    }
}