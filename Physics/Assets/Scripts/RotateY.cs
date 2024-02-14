using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RotateY : MonoBehaviour
{
    public float speed = 10.0f;
    public float minRotation;
    public float maxRotation = 45.0f;

    private Vector3 _currentEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        _currentEulerAngles = transform.localEulerAngles;
        minRotation += _currentEulerAngles.y;
        maxRotation += _currentEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        _currentEulerAngles.y += Time.deltaTime * speed;
        if (_currentEulerAngles.y >= maxRotation)
        {
            speed = -1 * Math.Abs(speed);
        }
        else if (_currentEulerAngles.y <= minRotation)
        {
            speed = Math.Abs(speed);
        }

        transform.localEulerAngles = _currentEulerAngles;
    }
}