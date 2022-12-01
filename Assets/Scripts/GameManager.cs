using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float _CompletionTreshold = .75f;
    [SerializeField] private float _CompletionTresholdJasmine = 1f;
    [SerializeField] private float _cutsceneCuePauseTime = 1f;

    private Dictionary<FlowerType, int> _totalFlowers
        = new Dictionary<FlowerType, int>();
    private Dictionary<FlowerType, int> _grownFlowers
        = new Dictionary<FlowerType, int>();
    private Dictionary<FlowerType, bool> _isAreaComplete
        = new Dictionary<FlowerType, bool>();

    private int _level = 0;
    private FlowerType _lastCompletedArea;
    private float _cutsceneCueLength;

    private UIManager _uiManager;
    private CameraManager _cameraManager;
    private PlayerVisuals _playerVisuals;
    private PlayerMovement _playerMovement;
    private RockGate[] _rockGates;

    public override void Awake()
    {
        base.Awake();
        _uiManager = GetComponent<UIManager>();
        _cameraManager = GetComponent<CameraManager>();
        _playerVisuals = FindObjectOfType<PlayerVisuals>();
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _rockGates = FindObjectsOfType<RockGate>();
        InitDictionaries();
        _cutsceneCueLength = _cutsceneCuePauseTime + _cameraManager.CameraMoveTime;
    }
    private void Start()
    {
        StartCoroutine(StartGameCutscene());
    }

    IEnumerator StartGameCutscene()
    {
        yield return null; // need this to avoid hanging on next yield in the Build

        _playerMovement.SetFreeze(true);
        _cameraManager.SetPlayerCloseUpCamera();
        StartCoroutine(_uiManager.PlayStartTexts());

        yield return new WaitForSeconds(_cutsceneCueLength);
        _cameraManager.SetPlayerCamera();
        _playerMovement.SetFreeze(false);
    }

    IEnumerator AreaCompleteCutscene()
    {
        Debug.Log($"Area {_lastCompletedArea} completed!");
        _isAreaComplete[_lastCompletedArea] = true;

        _uiManager.PlayFlowerText(_lastCompletedArea);
        _playerVisuals.PlayCelebrateAnimation();
        _playerMovement.SetFreeze(true);
        _cameraManager.SetHighCamera(_lastCompletedArea);

        if (_lastCompletedArea == FlowerType.Jasmine)
        {
            yield return EndGameCutscene();
            yield break;
        }

        yield return new WaitForSeconds(_cutsceneCueLength);
        _playerVisuals.ActivateFlower(_lastCompletedArea);
        ActivateRockGate();
        _cameraManager.SetGateCamera(_lastCompletedArea);

        yield return new WaitForSeconds(_cutsceneCueLength);
        _cameraManager.SetPlayerCloseUpCamera();

        yield return new WaitForSeconds(_cutsceneCueLength);
        _cameraManager.SetPlayerCamera();
        _playerMovement.SetFreeze(false);

        if (_lastCompletedArea == FlowerType.Rose)
            StartCoroutine(_uiManager.PlayArea1Texts());
        if (_lastCompletedArea == FlowerType.Lavender)
            StartCoroutine(_uiManager.PlayArea5Texts());
    }

    IEnumerator EndGameCutscene()
    {
        yield return new WaitForSeconds(_cutsceneCueLength);
        _playerVisuals.ActivateFlower(_lastCompletedArea);

        yield return new WaitForSeconds(_cutsceneCueLength);
        StartCoroutine(_uiManager.PlayEndTexts());

        yield return new WaitForSeconds(_cutsceneCueLength);
        _cameraManager.SetPlayerCamera();
        _playerMovement.SetFreeze(false);
    }

    public bool IsAreaComplete(FlowerType type) => _isAreaComplete[type];

    public void OnGrowFlower(FlowerType type)
    {
        _grownFlowers[type]++;
        Debug.Log($"Growing flower {_grownFlowers[type]} of {_totalFlowers[type]} for type {type}");

        if (!_isAreaComplete[type] && AreaCompleteConditions(type))
        {
            _lastCompletedArea = type;
            PrepareLevelIncrease();
        }

    }

    public void OnMusicChanged(int level)
    {
        if (level != _level)
        {
            _level = level;
            StartCoroutine(AreaCompleteCutscene());
        }
    }

    private void PrepareLevelIncrease()
    {
        Debug.Log($"Completion treshold reached for area {_lastCompletedArea}.");

        MusicManager.Instance.Level = _level + 1;
    }

    private void ActivateRockGate()
    {
        foreach (RockGate rockGate in _rockGates)
            rockGate.Sink(_lastCompletedArea);
    }

    private bool AreaCompleteConditions(FlowerType type)
    {
        return (type != FlowerType.Jasmine && (float)_grownFlowers[type] / _totalFlowers[type] >= _CompletionTreshold)
            || (type == FlowerType.Jasmine && (float)_grownFlowers[type] / _totalFlowers[type] >= _CompletionTresholdJasmine);
    }

    private void InitDictionaries()
    {
        foreach (FlowerType type in Enum.GetValues(typeof(FlowerType)))
        {
            _grownFlowers.Add(type, 0);
            _totalFlowers.Add(type, 0);
            _isAreaComplete.Add(type, false);
        }
        foreach (Flower flower in FindObjectsOfType<Flower>())
            _totalFlowers[flower.Type]++;
    }
}
