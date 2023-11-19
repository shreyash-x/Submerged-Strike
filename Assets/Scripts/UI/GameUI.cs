using TMPro;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI mineText;
        [SerializeField] private GameObject radar;

        public void SetCoins(int coins)
        {
            coinText.SetText(coins.ToString());
        }

        public void SetTime(string time)
        {
            timeText.SetText(time);
        }

        public void SetMinesDestroyed(int minesDestroyed, int totalMines)
        {
            if (totalMines > 1000)
            {
                mineText.SetText($"{minesDestroyed}");
                return;
            }
            mineText.SetText($"{minesDestroyed}/{totalMines}");
        }

        public void ToggleRadar(bool enable)
        {
            radar.SetActive(enable);
        }
    }
}