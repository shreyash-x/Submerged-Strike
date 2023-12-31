﻿using Game;
using Game.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button keyboardBtn;
        [SerializeField] private Button joystickBtn;
        [SerializeField] private Toggle playTutorialToggle;
        [SerializeField] private Toggle radarToggle;
        [SerializeField] private Toggle iconsToggle;

        [SerializeField] private Sprite keyboardOnSprite;
        [SerializeField] private Sprite keyboardOffSprite;
        
        [SerializeField] private Sprite joystickOnSprite;
        [SerializeField] private Sprite joystickOffSprite;

        // TextMeshProUGUI Dropdown
        [SerializeField] private TMP_Dropdown difficultyDropdown;

        [SerializeField] private DataManager dataManager;

        private void Start()
        {
            playTutorialToggle.SetIsOnWithoutNotify(dataManager.PlayTutorial);
            playTutorialToggle.onValueChanged.AddListener((val) =>
            {
                dataManager.PlayTutorial = val;
            });

            radarToggle.SetIsOnWithoutNotify(dataManager.EnableRadar);
            radarToggle.onValueChanged.AddListener((val) =>
            {
                dataManager.EnableRadar = val;
            });

            iconsToggle.SetIsOnWithoutNotify(dataManager.EnableIcons);
            iconsToggle.onValueChanged.AddListener((val) =>
            {
                dataManager.EnableIcons = val;
            });

            difficultyDropdown.value = (int)dataManager.Difficulty;
            difficultyDropdown.onValueChanged.AddListener((val) =>
            {
                dataManager.Difficulty = (int)val;
            });
            
            if (dataManager.InputMode == InputMode.Joystick)
            {
                keyboardBtn.image.sprite = keyboardOffSprite;
                joystickBtn.image.sprite = joystickOnSprite;
            }
            else
            {
                keyboardBtn.image.sprite = keyboardOnSprite;
                joystickBtn.image.sprite = joystickOffSprite;
            }
            
            keyboardBtn.onClick.AddListener(() =>
            {
                keyboardBtn.image.sprite = keyboardOnSprite;
                joystickBtn.image.sprite = joystickOffSprite;

                dataManager.InputMode = InputMode.Keyboard;
            });
            
            joystickBtn.onClick.AddListener(() =>
            {
                keyboardBtn.image.sprite = keyboardOffSprite;
                joystickBtn.image.sprite = joystickOnSprite;

                dataManager.InputMode = InputMode.Joystick;
            });

            volumeSlider.value = dataManager.EffectsVolume;
            volumeSlider.onValueChanged.AddListener((volume) =>
            {
                dataManager.EffectsVolume = volume;
                AudioListener.volume = volume;
            });
        }
    }
}