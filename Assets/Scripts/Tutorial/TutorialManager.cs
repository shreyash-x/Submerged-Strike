using System;
using Game;
using UnityEngine;

namespace Tutorial
{
    public abstract class TutorialStep : MonoBehaviour
    {
        [SerializeField] protected GameManager gameManager;

        public event Action End;
        public abstract void Begin();

        protected virtual void OnEnd()
        {
            End?.Invoke();
        }
    }
    
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialStep[] steps;
        
        public event Action TutorialComplete;
        
        private int _currentStep = 0;

        public void StartTutorial()
        {
            foreach (var step in steps)
            {
                step.End += () =>
                {
                    if (_currentStep + 1 >= steps.Length)
                    {
                        TutorialComplete?.Invoke();
                        return;
                    }
                    steps[_currentStep + 1].Begin();
                    _currentStep++;
                };
            }
            steps[0].Begin();
        }
    }
}
