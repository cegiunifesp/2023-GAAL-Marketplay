using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class Shelf : MonoBehaviour
{
    private const int MaxBehindProducts = 4;
    private const int MaxFrontProducts = 5;
    [SerializeField] private int _frontalProductsCount;
    [SerializeField] private int _behindProductsCount;

    private Dictionary<string, int> _ordersAmountLeft;


    [SerializeField] private int _index;

    [SerializeField] private Transform _frontalProductsParent;
    [SerializeField] private Transform _behindProductsParent;
    [SerializeField] private GameObject _correctImage;
    [SerializeField] private GameObject _incorrectImage;

    [SerializeField] private Button _confirmBt;

    [SerializeField] private List<Order> _orders;

    private void Start()
    {
        _confirmBt.onClick.AddListener(CheckProductsInShelf);
    }

    public int GetIndex() => _index;

    public void SetOrders(List<Order> orders)
    {
        _orders = orders;
        _ordersAmountLeft = new Dictionary<string, int>();
        foreach(Order order in _orders)
        {
            _ordersAmountLeft.Add(order.ProductName, order.ProductAmount);
        }
    }

    private void CheckProductsInShelf()
    {
        for (int i = 0; i < _ordersAmountLeft.Count; i++)
        {
            if (_ordersAmountLeft[_orders[i].ProductName] != 0)
            {
                ChangeCorrectImage(correct: false);
                RemoveProductsFromShelf();

                return;
            }
        }

        ChangeCorrectImage(correct: true);
        ConfirmCorrectProducts();
    }


    public bool AddProduct(ProductLevel2 product)
    {
        if (_behindProductsCount == MaxBehindProducts)
        {
            if (_frontalProductsCount == MaxFrontProducts)
            {
                print("Shelf is full");
                return false;
            }
            product.transform.SetParent(_frontalProductsParent);
            product.AddedToShelf(_frontalProductsParent.GetComponent<RectTransform>().rect.height);
            _frontalProductsCount++;
        }
        else
        {
            product.transform.SetParent(_behindProductsParent);
            product.AddedToShelf(_behindProductsParent.GetComponent<RectTransform>().rect.height);
            _behindProductsCount++;
        }

        DiscountProductAmount(product);
        return true;
    }

    public void ProductReplaced(string productRemoved)
    {
        _frontalProductsCount = _frontalProductsParent.childCount;
        _behindProductsCount = _behindProductsParent.childCount;

        if (_ordersAmountLeft.ContainsKey(productRemoved)) _ordersAmountLeft[productRemoved]++;

        while (_behindProductsCount < MaxBehindProducts)
        {
            if (_frontalProductsCount > 0)
            {
                Transform product = _frontalProductsParent.GetChild(_frontalProductsCount - 1);
                product.SetParent(_behindProductsParent);
                product.GetComponent<ProductLevel2>().AddedToShelf(_behindProductsParent.GetComponent<RectTransform>().rect.height);
                _behindProductsCount++;
                _frontalProductsCount--;
            }
            else break;
        }
    }

    private void DiscountProductAmount(ProductLevel2 product)
    {
        if (_ordersAmountLeft == null) return;

        if (_ordersAmountLeft.ContainsKey(product.ProductName))
        {
            _ordersAmountLeft[product.ProductName]--;
        }
    }

    private void ConfirmCorrectProducts()
    {
        _confirmBt.interactable = false;

        foreach (Transform child in _frontalProductsParent)
        {
            child.GetComponent<ProductLevel2>().SetInteractable(false);
        }
        foreach (Transform child in _behindProductsParent)
        {
            child.GetComponent<ProductLevel2>().SetInteractable(false);
        }

        Events.Instance.OnAddScore(500);
        Events.Instance.OnShelfCompleted(_index);
    }

    private void RemoveProductsFromShelf()
    {
        int i;
        while (_frontalProductsCount != 0 || _behindProductsCount != 0 )
        {
            for (i = 0; i < _frontalProductsParent.childCount; i++)
            {
                _frontalProductsParent.GetChild(0).GetComponent<ProductLevel2>().ShelfRemovedProduct();
            }

            for (i = 0; i < _behindProductsParent.childCount; i++)
            {
                _behindProductsParent.GetChild(0).GetComponent<ProductLevel2>().ShelfRemovedProduct();
            }

            _frontalProductsCount = _frontalProductsParent.childCount;
            _behindProductsCount = _behindProductsParent.childCount;
        }

        for (i = 0; i < _ordersAmountLeft.Count; i++)
        {
            _ordersAmountLeft[_orders[i].ProductName] = _orders[i].ProductAmount;
        }

        Events.Instance.OnRemoveScore(100);
    }

    private async void ChangeCorrectImage(bool correct)
    {
        if (correct)
        {
            _correctImage.SetActive(true);
            return;
        }

        _incorrectImage.SetActive(true);

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, destroyCancellationToken);
        //await Task.Delay(1000);

        _incorrectImage.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            collision.gameObject.GetComponent<ProductLevel2>().SetShelfReferenceWhenDragging(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            collision.gameObject.GetComponent<ProductLevel2>().ClearShelfReferenceWhenDragging(this);
        }
    }
}
