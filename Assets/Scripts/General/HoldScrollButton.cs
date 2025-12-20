using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class HoldScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _amount;
    [SerializeField] private Scrollbar _scroll;
    private bool _clicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        _clicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _clicked = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (_clicked)
        {
            _scroll.value += Time.deltaTime * _amount;
        }
    }
}
