using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _hoverClip;

    private Selectable component;

    // Start is called before the first frame update
    void Start()
    {
        component = GetComponent<Selectable>();
    }


    public void GetClips(AudioClip click, AudioClip hover)
    {
        _clickClip = click;
        _hoverClip = hover;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!component.interactable || _clickClip == null) return;

        AudioManager.OnPlayUI(_clickClip, 0.6f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!component.interactable || _hoverClip == null) return;

        AudioManager.OnPlayUI(_hoverClip, 0.2f);
    }

}
