using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ShoppingCart : MonoBehaviour
{
    [SerializeField] protected Image[] itemPlaces;

    private int placeIndex = 0;

    internal void AddItem(ProductBase product)
    {
        itemPlaces[placeIndex].sprite = product.ProductSprite;
        itemPlaces[placeIndex].rectTransform.sizeDelta = product.GetSizeVector2();
        itemPlaces[placeIndex].color = Color.white;

        placeIndex++;
    }
}
