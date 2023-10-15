using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Basket : MonoBehaviour
{
    [SerializeField] private List<Transform> _productsParents;
    [SerializeField] private Button _confirmBt;
    [SerializeField] private Button _declaineBt;

    private int _currentParentIndex;
    private int _wrongProducts;
    private bool _hasWrongProduct;

    private Dictionary<string, int> _ordersAmount;
    private List<Order> _orders;

    private void Start()
    {
        _confirmBt.onClick.AddListener(ConfirmProducts);
        _declaineBt.onClick.AddListener(RemoveProductsFromBasket);
    }

    public void SetOrders(List<Order> orders)
    {
        _orders = orders;

        _ordersAmount = new Dictionary<string, int>();
        foreach (Order order in orders)
        {
            _ordersAmount.Add(order.ProductName, order.ProductAmount);
        }
    }

    public void AddProduct(ProductBase product)
    {
        Transform t = product.transform;
        t.SetParent(_productsParents[_currentParentIndex]);
        t.localPosition = Vector3.zero;
        product.Size = 80;
        //t.position = transform.position;

        DiscountProductAmount(product);

        _currentParentIndex = (_currentParentIndex + 1) % _productsParents.Count;
    }

    private void DiscountProductAmount(ProductBase product)
    {
        if (_ordersAmount == null) return;

        if (_ordersAmount.ContainsKey(product.ProductName))
        {
            _ordersAmount[product.ProductName]--;
            //print($"Add {product.ProductName} Left: {_ordersAmount[product.ProductName]}");
        }
        else
        {
            _hasWrongProduct = true;
            _wrongProducts++;
        }
    }

    private void ConfirmProducts()
    {
        if (AllProductsCorrect())
        {
            LeanTween.moveY(gameObject, transform.position.y + 0.5f, 0.25f).setEaseInQuad().setLoopPingPong(1);
            Events.Instance.OnAddScore(500);
            Events.Instance.OnGameEnded();
            return;
        }

        transform.eulerAngles = Vector3.forward * 10;
        LeanTween.rotateZ(gameObject, -10, 0.2f).setLoopPingPong(1).setOnComplete(() =>
        {
            LeanTween.rotateZ(gameObject, 0, 0.2f);
        });
    }

    private bool AllProductsCorrect()
    {
        if (_ordersAmount == null) return false;

        if (_hasWrongProduct)
        {
            _hasWrongProduct = false;
            RemoveProductsFromBasket();
            Events.Instance.OnRemoveScore(30);
            return false;
        }

        foreach (var item in _ordersAmount)
        {
            if (item.Value != 0)
            {
                RemoveProductsFromBasket();
                Events.Instance.OnRemoveScore(30);
                return false;
            }
        }

        return true;
    }

    private void RemoveProductsFromBasket()
    {
        foreach (Transform parent in _productsParents)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var product = parent.GetChild(i).GetComponent<ProductLevel3>();
                if (_ordersAmount.ContainsKey(product.ProductName)) _ordersAmount[product.ProductName]++;
                product.RemoveFromBasket();
                _wrongProducts--;

                if(_wrongProducts == 0)
                {
                    _hasWrongProduct = false;
                }
            }
        }
        _currentParentIndex = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            var product = collision.gameObject.GetComponent<ProductLevel3>();
            product.SetBasket(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject objectCollided = collision.gameObject;
        if (objectCollided.CompareTag("Product"))
        {
            var product = objectCollided.GetComponent<ProductLevel3>();
            
            if (product.IsDragging())
            {
                if (_ordersAmount.ContainsKey(product.ProductName)) _ordersAmount[product.ProductName]++;
                else
                {
                    if (_wrongProducts <= 1)
                    {
                        _hasWrongProduct = false;
                        _wrongProducts = 0;
                    }
                    else
                    {
                        _wrongProducts--;
                    }
                }
                product.ClearBasketReference();
            }
        }
    }
}
