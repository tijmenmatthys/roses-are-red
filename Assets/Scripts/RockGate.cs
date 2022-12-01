using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGate : MonoBehaviour
{
    [SerializeField] private FlowerType _activatedBy;

    public void Sink(FlowerType activatingFlower)
    {
        if (_activatedBy == activatingFlower)
            GetComponentInChildren<Animator>().SetTrigger("Sink");
    }
}
