using TMPro;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timeText;

        public void SetCoins(int coins)
        {
            coinText.SetText(coins.ToString());
        }

        public void SetTime(string time)
        {
            timeText.SetText(time);
        }
    }
}