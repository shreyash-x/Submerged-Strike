using UnityEngine;

namespace UI
{
    public class ResponsiveGameUIManager : MonoBehaviour
    {
        [SerializeField] private GameUI gamePortraitUI;
        [SerializeField] private GameUI gameLandscapeUI;
        [SerializeField] private DeadMenu deadPortraitMenu;
        [SerializeField] private DeadMenu deadLandscapeMenu;

        private static bool IsPortrait => Screen.height >= Screen.width;

        public GameUI GameUI => IsPortrait ? gamePortraitUI : gameLandscapeUI;
        public DeadMenu DeadMenu => IsPortrait ? deadPortraitMenu : deadLandscapeMenu;

        private bool _isPortrait;

        private void Awake()
        {
            _isPortrait = IsPortrait;
            gamePortraitUI.gameObject.SetActive(_isPortrait);
            gameLandscapeUI.gameObject.SetActive(!_isPortrait);
            
            deadPortraitMenu.gameObject.SetActive(false);
            deadLandscapeMenu.gameObject.SetActive(false);
        }
    }
}