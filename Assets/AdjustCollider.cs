using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCollider : MonoBehaviour
{
    [SerializeField] private RectTransform _rectCollider;
    [SerializeField] private BoxCollider2D _collider;

    void Start()
    {
        if (_collider == null) _collider = GetComponent<BoxCollider2D>();
        if (_rectCollider == null) _rectCollider = GetComponent<RectTransform>();

        _collider.size = new Vector2(_rectCollider.rect.width, _rectCollider.rect.height);
    }

}
