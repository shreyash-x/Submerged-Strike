using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarMain : MonoBehaviour
{
    [SerializeField] private Transform radarPingPrefab;
    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radarRadius;
    private Transform sweepTransform;
    private List<Collider2D> targets;
    private GameObject _player;

    private void Awake()
    {
        sweepTransform = transform.Find("Sweep");
        targets = new List<Collider2D>();
        _player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        float previousRotation = (sweepTransform.eulerAngles.z % 360) - 180;
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationSpeed * Time.deltaTime);
        float currentRotation = (sweepTransform.eulerAngles.z % 360) - 180;

        if (previousRotation < 0 && currentRotation >= 0)
        {
            targets.Clear();
        }

        RaycastHit2D[] raycastHitList = Physics2D.RaycastAll(transform.position, Utility.GetVectorFromAngle(sweepTransform.eulerAngles.z), 100f, radarLayerMask);
        foreach (RaycastHit2D raycastHit in raycastHitList)
        {
            if (raycastHit.collider != null)
            {
                if (!targets.Contains(raycastHit.collider))
                {
                    targets.Add(raycastHit.collider);
                    RadarPing radarPing = Instantiate(radarPingPrefab, raycastHit.point, Quaternion.identity).GetComponent<RadarPing>();
                    if (raycastHit.collider.IsPartOfLayer("Missile"))
                    {
                        radarPing.SetColor(Color.red);
                    }
                    else if (raycastHit.collider.IsPartOfLayer("Mine"))
                    {
                        radarPing.SetColor(Color.green);
                    }
                    else if (raycastHit.collider.IsPartOfLayer("Coin"))
                    {
                        radarPing.SetColor(Color.yellow);
                    }

                    radarPing.SetDisappearTimer(360.0f / rotationSpeed);
                }
            }
        }
    }

    private void LateUpdate()
    {
        // Set position so that it's always on the player
        if (_player != null)
            transform.position = _player.transform.position;
        else
            _player = GameObject.FindWithTag("Player");
    }
}
