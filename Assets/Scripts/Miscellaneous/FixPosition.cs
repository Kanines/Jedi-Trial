using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour
{
    private Vector3 _initPosition;
    void Awake()
    {
        _initPosition = transform.position;
    }
    void LateUpdate()
    {
        transform.position = _initPosition;
    }
}
