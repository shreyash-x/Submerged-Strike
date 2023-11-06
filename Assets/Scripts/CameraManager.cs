using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float landscapeCameraSize;
    [SerializeField] private float portraitCameraSize;
    [SerializeField] private CinemachineVirtualCamera camera;

    private void Awake()
    {
        camera.m_Lens.OrthographicSize = Screen.width >= Screen.height ? landscapeCameraSize : portraitCameraSize;
    }
}