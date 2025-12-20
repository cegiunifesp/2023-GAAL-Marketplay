using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

[Serializable]
public class Order
{
    [field: SerializeField] public string ProductName { get; private set; }
    [field: SerializeField] public int ProductAmount { get; private set; }

    public Sprite ProductSprite;
    //public string Url { get; private set; }

    public Order(string productName, int amount, Sprite sprite)
    {
        ProductName = productName;
        ProductAmount = amount;
        ProductSprite = sprite;
        //Url = clip;
    }
}
