using System;
using Game;
using UnityEngine;

namespace Narrative
{
    public abstract class NarrativeStep : MonoBehaviour
    {
        [SerializeField] protected GameManager gameManager;

        public event Action End;
        public abstract void Begin();

        protected virtual void OnEnd()
        {
            End?.Invoke();
        }
    }
    
    public class NarrativeManager : MonoBehaviour
    {
        [SerializeField] private NarrativeStep[] steps;
        [SerializeField] private NarrativeStep[] finalSteps;
        
        public event Action NarrativeComplete;
        public event Action GameComplete;
        
        private int _currentStep = 0;
        private int _currentFinalStep = 0;

        public void StartNarrative()
        {
            foreach (var step in steps)
            {
                step.End += () =>
                {
                    if (_currentStep + 1 >= steps.Length)
                    {
                        NarrativeComplete?.Invoke();
                        return;
                    }
                    steps[_currentStep + 1].Begin();
                    _currentStep++;
                };
            }
            if (steps.Length > 0)
                steps[0].Begin();
            else
                NarrativeComplete?.Invoke();
        }

        public void StartGameComplete()
        {
            foreach (var step in finalSteps)
            {
                step.End += () =>
                {
                    if (_currentFinalStep + 1 >= finalSteps.Length)
                    {
                        GameComplete?.Invoke();
                        return;
                    }
                    finalSteps[_currentFinalStep + 1].Begin();
                    _currentFinalStep++;
                };
            }
            if (finalSteps.Length > 0)
                finalSteps[0].Begin();
            else
                GameComplete?.Invoke();
        }
    }
}
