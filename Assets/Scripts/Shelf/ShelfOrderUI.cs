using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShelfOrderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shelfIndexTx;
    [SerializeField] private Transform _ordersParent;
    [SerializeField] private OrderUI _orderPrefab;

    private Sprite[] _numbersSprite;

    public bool Initiated { get; private set; }

    public void Initiate(int index, Order[] orders, Sprite[] numbers)
    {
        Initiated = true;
        _shelfIndexTx.text = $"Prateleira {index}";
        _numbersSprite = numbers;

        LeanTween.scale(gameObject, Vector3.one, 0.75f).setEaseOutQuart();

        InstantiateOrders(orders);
    }

    private void InstantiateOrders(Order[] orders)
    {
        foreach(Order order in orders)
        {
            var newOrder = Instantiate(_orderPrefab, _ordersParent);
            newOrder.Initiate(_numbersSprite[order.ProductAmount], order.ProductSprite);
        }
    }
}
