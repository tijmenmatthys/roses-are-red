using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTransform : MonoBehaviour
{
    [SerializeField] float _minSize = .9f;
    [SerializeField] float _maxSize = 1.1f;

    [SerializeField] float _maxRotationY = 180;
    [SerializeField] float _maxRotationXZ = 10;

    void Start()
    {
        float size = Random.Range(_minSize, _maxSize);
        float rotationX = Random.Range(-_maxRotationXZ, _maxRotationXZ);
        float rotationY = Random.Range(-_maxRotationY, _maxRotationY);
        float rotationZ = Random.Range(-_maxRotationXZ, _maxRotationXZ);

        transform.localScale = Vector3.one * size;
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
    }
}
