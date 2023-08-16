using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterboxAdjustment : MonoBehaviour
{
    private void Start()
    {
        AdjustLetterboxBars();
    }

    private void AdjustLetterboxBars()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        float targetAspectRatio = mainCamera.aspect;
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio > targetAspectRatio)
        {
            float scaleFactor = targetAspectRatio / currentAspectRatio;
            transform.localScale = new Vector3(scaleFactor, 1f, 1f);
        }
    }
}






