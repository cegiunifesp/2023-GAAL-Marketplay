using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpProducts : MonoBehaviour
{
    private bool _startSpinning;

    private void Update()
    {
        if (!_startSpinning) return;

        transform.Rotate(Vector3.back * 40 * Time.deltaTime);
    }

    private void OnEnable()
    {
        _startSpinning = true;
    }
}
