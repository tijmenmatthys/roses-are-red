using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float _cameraMoveTime = 3f;
    [SerializeField] private CinemachineVirtualCamera _playerCam;
    [SerializeField] private CinemachineVirtualCamera _playerCloseUpCam;

    [Space]
    [SerializeField] private CinemachineVirtualCamera _roseHighCam;
    [SerializeField] private CinemachineVirtualCamera _violetHighCam;
    [SerializeField] private CinemachineVirtualCamera _sunflowerHighCam;
    [SerializeField] private CinemachineVirtualCamera _marigoldHighCam;
    [SerializeField] private CinemachineVirtualCamera _lavenderHighCam;
    [SerializeField] private CinemachineVirtualCamera _jasmineHighCam;

    [Space]
    [SerializeField] private CinemachineVirtualCamera _roseGateCam;
    [SerializeField] private CinemachineVirtualCamera _violetGateCam;
    [SerializeField] private CinemachineVirtualCamera _sunflowerGateCam;
    [SerializeField] private CinemachineVirtualCamera _marigoldGateCam;
    [SerializeField] private CinemachineVirtualCamera _lavenderGateCam;

    private Dictionary<FlowerType, CinemachineVirtualCamera> _highCameras
        = new Dictionary<FlowerType, CinemachineVirtualCamera>();
    private Dictionary<FlowerType, CinemachineVirtualCamera> _gateCameras
        = new Dictionary<FlowerType, CinemachineVirtualCamera>();

    private const int _lowPriority = 1;
    private const int _highPriority = 10;

    public float CameraMoveTime { get => _cameraMoveTime; }

    private void Start()
    {
        InitCameraDicts();
        SetCameraBlendTime();
    }

    public void SetPlayerCamera()
        => SetCamera(_playerCam);
    public void SetPlayerCloseUpCamera()
        => SetCamera(_playerCloseUpCam);

    public void SetHighCamera(FlowerType area)
        => SetCamera(_highCameras[area]);

    public void SetGateCamera(FlowerType area)
        => SetCamera(_gateCameras[area]);

    private void SetCamera(CinemachineVirtualCamera camera)
    {
        _playerCam.Priority = _lowPriority;
        _playerCloseUpCam.Priority = _lowPriority;
        foreach (var cam in _highCameras.Values)
            if (cam != null)
                cam.Priority = _lowPriority;
        foreach (var cam in _gateCameras.Values)
            if (cam != null)
                cam.Priority = _lowPriority;

        camera.Priority = _highPriority;
    }

    private void SetCameraBlendTime()
        => FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Time = CameraMoveTime;

    private void InitCameraDicts()
    {
        _highCameras.Add(FlowerType.Rose, _roseHighCam);
        _highCameras.Add(FlowerType.Violet, _violetHighCam);
        _highCameras.Add(FlowerType.Sunflower, _sunflowerHighCam);
        _highCameras.Add(FlowerType.Marigold, _marigoldHighCam);
        _highCameras.Add(FlowerType.Lavender, _lavenderHighCam);
        _highCameras.Add(FlowerType.Jasmine, _jasmineHighCam);

        _gateCameras.Add(FlowerType.Rose, _roseGateCam);
        _gateCameras.Add(FlowerType.Violet, _violetGateCam);
        _gateCameras.Add(FlowerType.Sunflower, _sunflowerGateCam);
        _gateCameras.Add(FlowerType.Marigold, _marigoldGateCam);
        _gateCameras.Add(FlowerType.Lavender, _lavenderGateCam);
    }
}
