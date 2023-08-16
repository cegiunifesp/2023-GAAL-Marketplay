using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotGizmo : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private bool _showGizmos;

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
