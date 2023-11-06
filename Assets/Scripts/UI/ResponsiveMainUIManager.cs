using UnityEngine;

namespace UI
{
    public class ResponsiveMainUIManager : MonoBehaviour
    {
        [SerializeField] private SettingsUI portraitSettingsUI;
        [SerializeField] private SettingsUI landscapeSettingsUI;

        [SerializeField] private StoreUI portraitStoreUI;
        [SerializeField] private StoreUI landscapeStoreUI;
        
        [SerializeField] private MainMenu portraitMainMenu;
        [SerializeField] private MainMenu landscapeMainMenu;

        [SerializeField] private FeedbackForm portraitFeedbackForm;
        [SerializeField] private FeedbackForm landscapeFeedbackForm;

        private static bool IsPortrait => Screen.height >= Screen.width;

        public SettingsUI SettingsUI => IsPortrait ? portraitSettingsUI : landscapeSettingsUI;
        public StoreUI StoreUI => IsPortrait ? portraitStoreUI : landscapeStoreUI;
        public MainMenu MainMenu => IsPortrait ? portraitMainMenu : landscapeMainMenu;
        public FeedbackForm FeedbackForm => IsPortrait ? portraitFeedbackForm : landscapeFeedbackForm;

        private bool _isPortrait;

        private void Awake()
        {
            _isPortrait = IsPortrait;
            portraitMainMenu.gameObject.SetActive(_isPortrait);
            landscapeMainMenu.gameObject.SetActive(!_isPortrait);
            
            portraitSettingsUI.gameObject.SetActive(false);
            landscapeSettingsUI.gameObject.SetActive(false);
            
            portraitStoreUI.gameObject.SetActive(false);
            landscapeStoreUI.gameObject.SetActive(false);
        }

        public void SetSettingsUIActive(bool active)
        {
            SettingsUI.gameObject.SetActive(active);
        }
        
        public void SetStoreUIActive(bool active)
        {
            StoreUI.gameObject.SetActive(active);
        }

        public void SetFormUIActive(bool active)
        {
            FeedbackForm.gameObject.SetActive(active);
        }
    }
}