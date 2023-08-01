using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class Order
{
    [field: SerializeField] public string ProductName { get; private set; }
    [field: SerializeField] public int ProductAmount { get; private set; }

    public Sprite ProductSprite;
    public VideoClip Clip { get; private set; }

    public Order(string productName, int amount, Sprite sprite, VideoClip clip)
    {
        ProductName = productName;
        ProductAmount = amount;
        ProductSprite = sprite;
        Clip = clip;
    }
}
