using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] private FlowerType _type;
    [SerializeField] private float _waveSpeedMultiplierMin = .9f;
    [SerializeField] private float _waveSpeedMultiplierMax = 1.1f;
    [SerializeField] GameObject _flower;
    [SerializeField] private Animator _animator;

    private int _playerLayer;
    private bool _hasGrown = false;

    public FlowerType Type { get { return _type; } }

    void Start()
    {
        _playerLayer = LayerMask.NameToLayer("Player");


        _flower.SetActive(false);
        RandomizeWaveSpeed();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!_hasGrown && other.gameObject.layer == _playerLayer)
            GrowFlower();
    }

    private void GrowFlower()
    {
        _flower.SetActive(true);
        _animator.SetTrigger("Grow");

        _hasGrown = true;
        GameManager.Instance.OnGrowFlower(_type);
    }
    private void RandomizeWaveSpeed()
    {
        float waveSpeedMultiplier = Random.Range(_waveSpeedMultiplierMin, _waveSpeedMultiplierMax);
        _animator.SetFloat("WaveSpeedMultiplier", waveSpeedMultiplier);
    }
}

public enum FlowerType
{
    Rose,
    Violet,
    Sunflower,
    Lavender,
    Marigold,
    Jasmine
}
