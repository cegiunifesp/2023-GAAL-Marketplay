using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Shelf : MonoBehaviour
{
    private const int MaxBehindProducts = 4;
    private const int MaxFrontProducts = 5;
    private int _frontalProductsCount;
    private int _behindProductsCount;

    private Dictionary<string, int> _ordersAmount;


    [SerializeField] private int _index;

    [SerializeField] private Transform _frontalProductsParent;
    [SerializeField] private Transform _behindProductsParent;
    [SerializeField] private TextMeshProUGUI _indexTx;

    [SerializeField] private Button _confirmBt;

    [SerializeField] private Color _correctColor;
    [SerializeField] private Color _wrongColor;

    [SerializeField] private List<Order> _orders;

    private void Start()
    {
        _confirmBt.onClick.AddListener(CheckProductsInShelf);
    }

    public int GetIndex() => _index;

    public void SetOrders(List<Order> orders)
    {
        _orders = orders;
        _ordersAmount = new Dictionary<string, int>();
        foreach(Order order in _orders)
        {
            _ordersAmount.Add(order.ProductName, order.ProductAmount);
        }
    }

    private void CheckProductsInShelf()
    {
        for (int i = 0; i < _ordersAmount.Count; i++)
        {
            if (_ordersAmount[_orders[i].ProductName] != 0)
            {
                ChangeIndexTextColor(correct: false);
                RemoveProductsFromShelf();

                return;
            }
        }

        ChangeIndexTextColor(correct: true);
        ConfirmCorrectProducts();
    }


    public void AddProduct(ProductLevel2 product)
    {
        if (_behindProductsCount == MaxBehindProducts)
        {
            if (_frontalProductsCount == MaxFrontProducts)
            {
                print("Shelf is full");
                return;
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
    }

    public void RemovedProduct(string productRemoved)
    {
        _frontalProductsCount = _frontalProductsParent.childCount;
        _behindProductsCount = _behindProductsParent.childCount;

        if (_ordersAmount.ContainsKey(productRemoved)) _ordersAmount[productRemoved]++;

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
        if (_ordersAmount == null) return;

        if (_ordersAmount.ContainsKey(product.ProductName))
        {
            _ordersAmount[product.ProductName]--;
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

        Events.Instance.OnShelfCompleted(_index);
        Events.Instance.OnAddScore(500);
    }

    private void RemoveProductsFromShelf()
    {
        foreach (Transform child in _frontalProductsParent)
        {
            var product = child.GetComponent<ProductLevel2>();
            if (_ordersAmount.ContainsKey(product.ProductName)) _ordersAmount[product.ProductName]++;
            product.RemoveFromShelf();

            _frontalProductsCount--;
        }
        foreach (Transform child in _behindProductsParent)
        {
            var product = child.GetComponent<ProductLevel2>();
            if (_ordersAmount.ContainsKey(product.ProductName)) _ordersAmount[product.ProductName]++;
            product.RemoveFromShelf();

            _behindProductsCount--;
        }

        Events.Instance.OnRemoveScore(100);
    }

    private async void ChangeIndexTextColor(bool correct)
    {
        _indexTx.color = !correct ? Color.red : Color.green;

        if (correct) return;

        await Task.Delay(1000);

        _indexTx.color = Color.white;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            collision.gameObject.GetComponent<ProductLevel2>().SetShelfReference(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            collision.gameObject.GetComponent<ProductLevel2>().ClearShelfReference(this);
        }
    }
}
