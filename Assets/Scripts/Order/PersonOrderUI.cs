using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PersonOrderUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _people;
    [SerializeField] private Sprite[] _numbersSprite;

    [Space(10)]
    [SerializeField] private Image _personImgSource;
    [SerializeField] private Transform _ordersParent;
    [SerializeField] private OrderUI _orderPrefab;

    private readonly int _height = 297;
    private Dictionary<string, OrderUI> _orders = new Dictionary<string, OrderUI>();

    public void Initiate(Order[] orders)
    {
        AdjustImageSize();

        LeanTween.scale(gameObject, Vector2.zero, 0.01f).setOnComplete(() =>
        {
            LeanTween.scale(gameObject, Vector3.one, 0.75f).setEaseOutQuart();
        });


        InstantiateOrders(orders);
    }

    private void AdjustImageSize()
    {
        _personImgSource.sprite = _people.GetRandomValue();
        _personImgSource.SetNativeSize();

        var size = _personImgSource.rectTransform.sizeDelta;
        float proportion = size.x / size.y;
        _personImgSource.rectTransform.sizeDelta = new Vector2(_height * proportion, _height);
    }

    private void InstantiateOrders(Order[] orders)
    {
        foreach (Order order in orders)
        {
            var newOrder = Instantiate(_orderPrefab, _ordersParent);
            int numberIndex = Mathf.Clamp(order.ProductAmount, 0, _numbersSprite.Length - 1);
            newOrder.Initiate(_numbersSprite[numberIndex], order.ProductSprite);
            _orders.Add(order.ProductName, newOrder);
        }
    }

    public void DiscardProduct(string productName)
    {
        if (_orders.TryGetValue(productName, out OrderUI order))
        {
            order.DiscardProduct();
        }
    }

    public void LetAvailable(string productName)
    {
        if (_orders.TryGetValue(productName, out OrderUI order))
        {
            order.ProductAvailable();
        }
    }
}
