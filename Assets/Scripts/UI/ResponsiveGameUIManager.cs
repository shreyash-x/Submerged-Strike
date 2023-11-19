using UnityEngine;

namespace UI
{
    public class ResponsiveGameUIManager : MonoBehaviour
    {
        [SerializeField] private GameUI gamePortraitUI;
        [SerializeField] private GameUI gameLandscapeUI;
        [SerializeField] private DeadMenu deadPortraitMenu;
        [SerializeField] private DeadMenu deadLandscapeMenu;
        [SerializeField] private MissionCompletedMenu completedMenu;
        [SerializeField] private StoreUI landscapeStoreUI;

        private static bool IsPortrait => Screen.height >= Screen.width;

        public GameUI GameUI => IsPortrait ? gamePortraitUI : gameLandscapeUI;
        public DeadMenu DeadMenu => IsPortrait ? deadPortraitMenu : deadLandscapeMenu;
        public MissionCompletedMenu MissionCompletedMenu => completedMenu;
        public StoreUI StoreUI => landscapeStoreUI;

        private bool _isPortrait;

        private void Awake()
        {
            _isPortrait = IsPortrait;
            gamePortraitUI.gameObject.SetActive(_isPortrait);
            gameLandscapeUI.gameObject.SetActive(!_isPortrait);
            
            deadPortraitMenu.gameObject.SetActive(false);
            deadLandscapeMenu.gameObject.SetActive(false);

            completedMenu.gameObject.SetActive(false);
            landscapeStoreUI.gameObject.SetActive(false);
        }

        public void SetStoreUIActive(bool active)
        {
            StoreUI.gameObject.SetActive(active);
        }
    }
}