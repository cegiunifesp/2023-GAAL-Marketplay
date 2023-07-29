using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Order
{
    public string ProductName { get; private set; }
    public int ProductAmount { get; private set; }

    public VideoClip Clip { get; private set; }

    public Order(string productName, int amount, VideoClip clip)
    {
        ProductName = productName;
        ProductAmount = amount;
        Clip = clip;
    }
}
