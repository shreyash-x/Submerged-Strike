using System;
using System.Collections.Generic;
using UnityEngine;
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
        private Vector3 objectivePosition;
        

        private void OnEnable()
        {
            maxProgress = 1;
            currentProgress = 0.25f;
            // get a random point at a distance of 10000 from the player
            var angle = Random.Range(0, 360);
            var dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Player.transform.up;
            objectivePosition = Player.transform.position + dir * destinationDistance;
            var distance = dir * destinationDistance;
            var haltZone = Instantiate(haltZonePrefab);
            haltZone.transform.position = objectivePosition;
            haltZone.transform.localScale = new Vector3(haltZoneRadius, haltZoneRadius, 1);
            haltZone.SetActive(true);

            progressScale = (maxProgress - currentProgress) / distance.magnitude;
            progressBar.SetProgress(currentProgress);
        }

        private void Update()
        {
            if (Player == null) return;
            var distance = objectivePosition - Player.transform.position;
            currentProgress = maxProgress - distance.magnitude * progressScale;
            progressBar.SetProgress(currentProgress);
            
            if(distance.magnitude < haltZoneRadius)
            {
                Debug.Log("Objective Reached");
            }
        }
    }
}