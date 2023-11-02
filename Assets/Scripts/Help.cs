using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    [SerializeField] AudioClip click;
    [SerializeField] AudioClip hover;

    [ContextMenu("Execute")]
    private void Execute()
    {
        var buttons = FindObjectsOfType<Selectable>();
        foreach (var button in buttons)
        {
            if ( !button.TryGetComponent(out CustomButton comp))
            {
                comp = button.AddComponent<CustomButton>();
            }
            comp.GetClips(click, hover);
        }
    }
}
