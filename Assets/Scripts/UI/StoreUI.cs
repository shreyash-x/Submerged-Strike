using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StoreUI : MonoBehaviour
    {
        [SerializeField] private GameData gameData;
        [SerializeField] private DataManager dataManager;
        
        [Header("Plane UI")]
        [SerializeField] private Image planeImage;
        [SerializeField] private TextMeshProUGUI planeDescription;
        [SerializeField] private Button planePrevBtn;
        [SerializeField] private Button planeNextBtn;
        [SerializeField] private Button planeBuyEquipBtn;
        [SerializeField] private GameObject planeBuyDisplay;
        [SerializeField] private TextMeshProUGUI planeCostText;
        [SerializeField] private GameObject planeEquipDisplay;
        [SerializeField] private GameObject planeEquippedDisplay;

        [Header("Shield UI")]
        [SerializeField] private Image shieldImage;
        [SerializeField] private TextMeshProUGUI shieldDescription;
        [SerializeField] private Button shieldPrevBtn;
        [SerializeField] private Button shieldNextBtn;
        [SerializeField] private Button shieldBuyEquipBtn;
        [SerializeField] private GameObject shieldBuyDisplay;
        [SerializeField] private TextMeshProUGUI shieldCostText;
        [SerializeField] private GameObject shieldEquipDisplay;
        [SerializeField] private GameObject shieldEquippedDisplay;

        private int _currentPlaneIndex;
        private int _currentShieldIndex;
        
        private void Start()
        {
            _currentPlaneIndex = dataManager.EquippedPlane;
            _currentShieldIndex = dataManager.EquippedShield;
            
            SetPlane(_currentPlaneIndex);
            SetShield(_currentShieldIndex);

            planePrevBtn.onClick.AddListener(() =>
            {
                _currentPlaneIndex--;
                if (_currentPlaneIndex < 0) _currentPlaneIndex = gameData.planes.Count - 1;

                SetPlane(_currentPlaneIndex);
            });
            
            planeNextBtn.onClick.AddListener(() =>
            {
                _currentPlaneIndex++;
                if (_currentPlaneIndex >= gameData.planes.Count) _currentPlaneIndex = 0;

                SetPlane(_currentPlaneIndex);
            });
            
            planeBuyEquipBtn.onClick.AddListener(() => BuyEquipPlane(_currentPlaneIndex));
            
            shieldPrevBtn.onClick.AddListener(() =>
            {
                _currentShieldIndex--;
                if (_currentShieldIndex < 0) _currentShieldIndex = gameData.shields.Count - 1;

                SetShield(_currentShieldIndex);
            });
            
            shieldNextBtn.onClick.AddListener(() =>
            {
                _currentShieldIndex++;
                if (_currentShieldIndex >= gameData.shields.Count) _currentShieldIndex = 0;

                SetShield(_currentShieldIndex);
            });
            
            shieldBuyEquipBtn.onClick.AddListener(() => BuyEquipShield(_currentShieldIndex));
        }

        private void SetPlane(int i)
        {
            planeImage.sprite = gameData.planes[i].planeSprite;
            planeDescription.SetText(gameData.planes[i].description);

            planeCostText.SetText($"{gameData.planes[i].cost}");
            
            if (dataManager.HasBoughtPlane(i))
            {
                planeBuyDisplay.SetActive(false);
                if (dataManager.EquippedPlane == i)
                {
                    planeEquippedDisplay.SetActive(true);
                    planeEquipDisplay.SetActive(false);
                }
                else
                {
                    planeEquippedDisplay.SetActive(false);
                    planeEquipDisplay.SetActive(true);
                }
            }
            else
            {
                planeBuyDisplay.SetActive(true);
                planeEquipDisplay.SetActive(false);
                planeEquippedDisplay.SetActive(false);
            }
        }

        private void BuyEquipPlane(int i)
        {
            if (!dataManager.TryBuyPlane(i)) return;
            
            planeBuyDisplay.SetActive(false);
            if (dataManager.EquippedPlane == i)
            {
                planeEquippedDisplay.SetActive(true);
                planeEquipDisplay.SetActive(false);
            }
            else
            {
                planeEquippedDisplay.SetActive(false);
                planeEquipDisplay.SetActive(true);
            }
        }
        
        private void BuyEquipShield(int i)
        {
            if (!dataManager.TryBuyShield(i)) return;
            
            if (dataManager.EquippedShield == i)
            {
                shieldEquippedDisplay.SetActive(true);
                shieldEquipDisplay.SetActive(false);
            }
            else
            {
                shieldEquippedDisplay.SetActive(false);
                shieldEquipDisplay.SetActive(true);
            }
        }

        private void SetShield(int i)
        {
            shieldImage.sprite = gameData.shields[i].shieldSprite;
            shieldImage.material = gameData.shields[i].shieldMaterial;
            shieldDescription.SetText(gameData.shields[i].description);

            shieldCostText.SetText($"{gameData.shields[i].cost}");
            
            if (dataManager.HasBoughtShield(i))
            {
                shieldBuyDisplay.SetActive(false);
                if (dataManager.EquippedShield == i)
                {
                    shieldEquippedDisplay.SetActive(true);
                    shieldEquipDisplay.SetActive(false);
                }
                else
                {
                    shieldEquippedDisplay.SetActive(false);
                    shieldEquipDisplay.SetActive(true);
                }
            }
            else
            {
                shieldBuyDisplay.SetActive(true);
                shieldEquipDisplay.SetActive(false);
                shieldEquippedDisplay.SetActive(false);
            }
        }
    }
}