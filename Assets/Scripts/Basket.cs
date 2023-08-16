using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private Button _confirmBt;
    [SerializeField] private Button _declaineBt;

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

    public void AddProduct(ProductLevel3 product)
    {
        Transform t = product.transform;
        t.SetParent(transform);
        t.position = transform.position;

        DiscountProductAmount(product);
    }

    private void DiscountProductAmount(ProductLevel3 product)
    {
        if (_ordersAmount == null) return;

        if (_ordersAmount.ContainsKey(product.ProductName))
        {
            _ordersAmount[product.ProductName]--;
            print($"Add {product.ProductName} Left: {_ordersAmount[product.ProductName]}");
        }
        else _hasWrongProduct = true;
    }

    private void ConfirmProducts()
    {
        if (AllProductsCorrect())
        {
            LeanTween.moveY(gameObject, transform.position.y + 0.5f, 0.25f).setEaseInQuad().setLoopPingPong(1);
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

                //foreach (Order order in _orders)
                //{
                //    _ordersAmount[order.ProductName] = order.ProductAmount;
                //}
                Events.Instance.OnRemoveScore(30);
                return false;
            }
        }

        return true;
    }

    private void RemoveProductsFromBasket()
    {
        foreach (Transform child in transform)
        {
            var product = child.GetComponent<ProductLevel3>();
            if (_ordersAmount.ContainsKey(product.ProductName)) _ordersAmount[product.ProductName]++;
            product.RemoveFromBasket();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            collision.gameObject.GetComponent<ProductLevel3>().SetBasket(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject objectCollided = collision.gameObject;
        if (objectCollided.CompareTag("Product"))
        {
            objectCollided.GetComponent<ProductLevel3>().ClearBasketReference();
        }
    }
}
