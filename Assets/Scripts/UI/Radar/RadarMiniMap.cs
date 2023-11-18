using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarMiniMap : MonoBehaviour
{
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void LateUpdate()
    {
        if (_player != null)
        {
            Vector3 newPosition = _player.transform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            _player = GameObject.FindWithTag("Player");
        }
    }
}
