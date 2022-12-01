using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _flowerJasmine;
    [SerializeField] private GameObject _flowerLavender;
    [SerializeField] private GameObject _flowerMarigold;
    [SerializeField] private GameObject _flowerRose;
    [SerializeField] private GameObject _flowerSunflower;
    [SerializeField] private GameObject _flowerViolet;

    [SerializeField] private GameObject _seedJasmine;
    [SerializeField] private GameObject _seedLavender;
    [SerializeField] private GameObject _seedMarigold;
    [SerializeField] private GameObject _seedRose;
    [SerializeField] private GameObject _seedSunflower;
    [SerializeField] private GameObject _seedViolet;

    private void Start()
    {
        _flowerJasmine.SetActive(false);
        _flowerLavender.SetActive(false);
        _flowerMarigold.SetActive(false);
        _flowerRose.SetActive(false);
        _flowerSunflower.SetActive(false);
        _flowerViolet.SetActive(false);
    }

    public void ActivateFlower(FlowerType type)
    {
        if (type == FlowerType.Jasmine)
        {
            _flowerJasmine.SetActive(true);
            _seedJasmine.SetActive(false);
        }
        if (type == FlowerType.Lavender)
        {
            _flowerLavender.SetActive(true);
            _seedLavender.SetActive(false);
        }
        if (type == FlowerType.Marigold)
        {
            _flowerMarigold.SetActive(true);
            _seedMarigold.SetActive(false);
        }
        if (type == FlowerType.Rose)
        {
            _flowerRose.SetActive(true);
            _seedRose.SetActive(false);
        }
        if (type == FlowerType.Sunflower)
        {
            _flowerSunflower.SetActive(true);
            _seedSunflower.SetActive(false);
        }
        if (type == FlowerType.Violet)
        {
            _flowerViolet.SetActive(true);
            _seedViolet.SetActive(false);
        }
    }

    public void PlayCelebrateAnimation()
    {
        _animator.SetTrigger("Celebrate");
    }
}
