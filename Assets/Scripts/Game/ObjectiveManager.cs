using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;
using Random = UnityEngine.Random;

namespace Game.Objective
{
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private float destinationDistance;
        [SerializeField] private GameObject haltZonePrefab;
        [SerializeField] private float haltZoneRadius;
        internal float maxProgress;
        internal float currentProgress;
        internal float progressScale;
        internal GameObject Player;
        internal bool ReachHaltZone;
        internal bool DestroyMines;
        internal bool DodgeMissiles;
        internal int TimeToSurvive;
        internal int TotalMines;
        internal DataManager DataManager;
        private Vector3 objectivePosition;
        private float timeSurvived;
        public event Action CompleteLevel;


        private void OnEnable()
        {
            maxProgress = 1;
            // get a random point at a distance of 10000 from the player
            if (ReachHaltZone)
            {
                currentProgress = 0.25f;
                var angle = Random.Range(0, 360);
                var dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Player.transform.up;
                objectivePosition = Player.transform.position + dir * (destinationDistance * (DataManager.Difficulty + 1));
                var distance = dir * destinationDistance;
                var haltZone = Instantiate(haltZonePrefab);
                haltZone.transform.position = objectivePosition;
                haltZone.transform.localScale = new Vector3(haltZoneRadius, haltZoneRadius, 1);
                haltZone.SetActive(true);
                progressScale = (maxProgress - currentProgress) / distance.magnitude;
            }
            else if (DestroyMines)
            {
                currentProgress = 0.0f;
                var minesDestroyed = DataManager.MinesDestroyed;
                var minesToDestroy = TotalMines - minesDestroyed;
                progressScale = (maxProgress - currentProgress) / minesToDestroy;
            }
            else if (DodgeMissiles)
            {
                currentProgress = 0.0f;
                timeSurvived = 0.0f;
                progressScale = (maxProgress - currentProgress) / TimeToSurvive;
            }

            progressBar.SetProgress(currentProgress);
        }

        private void Update()
        {
            if (Player == null) return;

            if (ReachHaltZone)
            {
                var distance = objectivePosition - Player.transform.position;
                currentProgress = maxProgress - distance.magnitude * progressScale;
                progressBar.SetProgress(currentProgress);

                if (distance.magnitude < haltZoneRadius)
                {
                    Debug.Log("Objective Reached");
                }
            }
            else if (DestroyMines)
            {
                var minesDestroyed = DataManager.MinesDestroyed;
                var minesToDestroy = TotalMines - minesDestroyed;
                currentProgress = maxProgress - minesToDestroy * progressScale;
                progressBar.SetProgress(currentProgress);

                if (minesToDestroy <= 0)
                {
                    Debug.Log("Objective Reached");
                    CompleteLevel?.Invoke();
                }
            }
            else if (DodgeMissiles)
            {
                timeSurvived += Time.deltaTime;
                currentProgress = timeSurvived * progressScale;
                progressBar.SetProgress(currentProgress);

                if (timeSurvived >= TimeToSurvive)
                {
                    Debug.Log("Objective Reached");
                    CompleteLevel?.Invoke();
                }
            }
        }
    }
}