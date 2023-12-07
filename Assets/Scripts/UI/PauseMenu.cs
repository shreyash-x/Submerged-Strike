using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private ResponsiveGameUIManager uiManager;
        
        [Space]
        [SerializeField] private TextMeshProUGUI baseCoinText;
        [SerializeField] private TextMeshProUGUI totalCoinText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI collectedCoinText;

        public void Display(int timeCoins, int coinsCollected, int totalCoins)
        {
            int total = timeCoins + coinsCollected;
            
            baseCoinText.SetText($"+{timeCoins}");
            totalCoinText.SetText($"{total}");
            collectedCoinText.SetText($"+{coinsCollected}");
            
            coinText.SetText($"{totalCoins}");
                        
            uiManager.GameUI.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        public void UpdateTotalCoins(int totalCoins)
        {
            coinText.SetText($"{totalCoins}");
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            uiManager.GameUI.gameObject.SetActive(true);
        }
    }
}