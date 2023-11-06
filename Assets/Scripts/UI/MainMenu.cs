using Game.Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private DataManager dataManager;
        [SerializeField] private TextMeshProUGUI coinText;

        private void Start()
        {
            AudioListener.volume = dataManager.EffectsVolume;
            coinText.SetText($"{dataManager.Coins}");
        }
    }
}