using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Shelf : MonoBehaviour
{
    [SerializeField] private int _index;

    [SerializeField] private Transform _frontalProductsParent;
    [SerializeField] private Transform _behindProductsParent;
    [SerializeField] private GameObject _correctImage;
    [SerializeField] private GameObject _incorrectImage;

    [SerializeField] private Button _confirmBt;
    [SerializeField] private List<Order> _orders;

    [SerializeField] private AudioClip _correctItems;
    [SerializeField] private AudioClip _wrongItems;


    private const int MaxBehindProducts = 4;
    private const int MaxFrontProducts = 5;
    private int _frontalProductsCount;
    private int _behindProductsCount;

    private Dictionary<string, int> _ordersAmountLeft;

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
        if (!AllProductsCorrect())
        {
            ChangeCorrectImage(correct: false);
            RemoveProductsFromShelf();

            AudioManager.OnPlaySFX(_wrongItems);
            return;
        }

        ChangeCorrectImage(correct: true);
        ConfirmCorrectProducts();
    }

    private bool AllProductsCorrect()
    {
        if (_orders == null || _orders.Count == 0) return false;

        Dictionary<string, int> remaining = new Dictionary<string, int>();
        foreach (Order order in _orders)
        {
            remaining[order.ProductName] = order.ProductAmount;
        }

        int extraItems = 0;
        CountProductsOnParent(_behindProductsParent, remaining, ref extraItems);
        CountProductsOnParent(_frontalProductsParent, remaining, ref extraItems);

        if (extraItems != 0) return false;

        foreach (var leftover in remaining)
        {
            if (leftover.Value != 0) return false;
        }

        return true;
    }

    private static void CountProductsOnParent(Transform parent, Dictionary<string, int> remaining, ref int extraItems)
    {
        if (parent == null) return;

        for (int i = 0; i < parent.childCount; i++)
        {
            ProductLevel2 product = parent.GetChild(i).GetComponent<ProductLevel2>();
            if (product == null) continue;

            if (remaining.ContainsKey(product.ProductName))
            {
                remaining[product.ProductName]--;
            }
            else
            {
                extraItems++;
            }
        }
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

        AudioManager.OnPlaySFX(_correctItems);

        Events.Instance.OnAddScore(500);
        Events.Instance.OnShelfCompleted(_index);
    }

    private async void RemoveProductsFromShelf()
    {
        List<ProductLevel2> productsToReturn = new List<ProductLevel2>();
        CollectProducts(_frontalProductsParent, productsToReturn);
        CollectProducts(_behindProductsParent, productsToReturn);

        for (int i = 0; i < productsToReturn.Count; i++)
        {
            try
            {
                await UniTask.Delay(50, false, PlayerLoopTiming.Update, destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            ProductLevel2 product = productsToReturn[i];
            if (product == null) continue;

            product.ShelfRemovedProduct();
        }

        _frontalProductsCount = _frontalProductsParent != null ? _frontalProductsParent.childCount : 0;
        _behindProductsCount = _behindProductsParent != null ? _behindProductsParent.childCount : 0;

        if (_orders != null && _ordersAmountLeft != null)
        {
            foreach (Order order in _orders)
            {
                if (order == null) continue;
                _ordersAmountLeft[order.ProductName] = order.ProductAmount;
            }
        }

        if (Events.Instance != null)
        {
            Events.Instance.OnRemoveScore(100);
        }
    }

    private static void CollectProducts(Transform parent, List<ProductLevel2> products)
    {
        if (parent == null) return;

        for (int i = 0; i < parent.childCount; i++)
        {
            ProductLevel2 product = parent.GetChild(i).GetComponent<ProductLevel2>();
            if (product != null)
            {
                products.Add(product);
            }
        }
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
