using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ShoppingCart : MonoBehaviour
{
    [SerializeField] protected Image[] itemPlaces;

    private int _placeIndex = 0;

    internal void AddItem(ProductBase product)
    {
        itemPlaces[_placeIndex].sprite = product.ProductSprite;
        itemPlaces[_placeIndex].rectTransform.sizeDelta = product.GetSizeVector2();
        itemPlaces[_placeIndex].color = Color.white;

        _placeIndex++;
    }
}
