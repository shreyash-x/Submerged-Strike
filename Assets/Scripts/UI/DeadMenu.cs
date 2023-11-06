using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class DeadMenu : MonoBehaviour
    {
        [SerializeField] private ResponsiveGameUIManager uiManager;
        
        [Space]
        [SerializeField] private TextMeshProUGUI baseCoinText;
        [SerializeField] private TextMeshProUGUI totalCoinText;
        [SerializeField] private TextMeshProUGUI coinText;

        [Space]
        [SerializeField] private Button replayButton;
        
        private void Awake()
        {
            replayButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            });
        }

        public void Display(int timeCoins, int coinsCollected, int totalCoins)
        {
            int total = timeCoins + coinsCollected;
            
            baseCoinText.SetText($"+{timeCoins}");
            totalCoinText.SetText($"{total}");
            
            coinText.SetText($"{totalCoins}");
                        
            uiManager.GameUI.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void UpdateTotalCoins(int totalCoins)
        {
            coinText.SetText($"{totalCoins}");
        }
    }
}