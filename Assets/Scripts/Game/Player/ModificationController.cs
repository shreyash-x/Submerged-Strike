using System;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Player
{
    internal enum ModificationType
    {
        None,
        InvertX,
        InvertY,
        InvertXY,
        DisableShield
    }
    
    [Serializable]
    internal class Modification
    {
        public float probability;
        public float duration;
        public ModificationType type;
        public PlayerEffectPanelUI[] ui;
    }
    
    public class ModificationController : MonoBehaviour
    {
        [SerializeField] private Modification[] modifications;
        
        internal PlayerController PlayerController;
        internal GameObject Shield;
        private ModificationType _currentModificationType;
        private Modification _currentModification;

        private float _elapsed = 0, _duration = 0;

        private void Start()
        {
            PlayerController.CosmicRayHit += PlayerControllerOnCosmicRayHit;
        }

        private void Update()
        {
            if (_currentModificationType != ModificationType.None)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= _duration)
                {
                    ApplyModification(null);
                }
            }
            else
            {
                _elapsed = _duration = 0;
            }
        }

        private void PlayerControllerOnCosmicRayHit(PlayerController arg1, CosmicRay arg2)
        {
            if (_currentModificationType == ModificationType.None)
            {
                var random = Random.Range(0f, 1f);
                float sum = 0;
                foreach (var modification in modifications)
                {
                    sum += modification.probability;
                    if (random <= sum)
                    {
                        // apply modification
                        ApplyModification(modification);
                        break;
                    }
                }
            }
            else
            {
                foreach (var ui in _currentModification.ui)
                {
                    ui.ResetAndHide();
                }
                ApplyModification(null);
            }
        }

        private void ApplyModification(Modification modification)
        {
            _currentModification = modification;
            if (modification == null)
            {
                _currentModificationType = ModificationType.None;
            }
            else
            {
                _currentModificationType = modification.type;
                _elapsed = 0;
                _duration = modification.duration;
                // modification.ui.ShowForSeconds(modification.duration);
                foreach (var ui in _currentModification.ui)
                {
                    ui.ShowForSeconds(modification.duration);
                }
            }
            switch (_currentModificationType)
            {
                case ModificationType.None:
                    PlayerController.XMultiplier = PlayerController.YMultiplier = 1;
                    if (Shield)
                    {
                        Shield.SetActive(true);
                    }
                    break;
                case ModificationType.InvertX:
                    PlayerController.XMultiplier = -1;
                    break;
                case ModificationType.InvertY:
                    PlayerController.YMultiplier = -1;
                    break;
                case ModificationType.InvertXY:
                    PlayerController.XMultiplier = -1;
                    PlayerController.YMultiplier = -1;
                    break;
                case ModificationType.DisableShield:
                    if (Shield)
                    {
                        Shield.SetActive(false);
                    }
                    break;
                default:
                    Debug.LogError($"Unsupported Modification Type {_currentModificationType}");
                    break;
            }
        }
    }
}