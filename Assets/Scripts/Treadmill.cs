using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [SerializeField] private float _limitX = 260;
    [SerializeField] private float _speedRolling = 1;

    [SerializeField] private bool _loading;
    [SerializeField] private Transform _child;

    private bool _stopRolling;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = _child.position;
        if (!_loading) Events.Instance.onGameEnded += HandleGameEnded;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopRolling) return;

        _child.position += Vector3.right * Time.deltaTime * _speedRolling;
        if (_child.localPosition.x > _limitX)
        {
            _child.position = _initialPosition;
        }
    }

    public void HandleGameEnded()
    {
        _stopRolling = true;
    }
}
