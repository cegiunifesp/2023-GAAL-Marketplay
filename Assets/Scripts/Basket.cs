using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using AYellowpaper.SerializedCollections;

public class Basket : MonoBehaviour
{
    [SerializeField] private List<Transform> _productsParents;
    [SerializeField] private PersonOrderUI _personOrderUI;
    [SerializeField] private Button _confirmBt;
    [SerializeField] private Button _declaineBt;

    [SerializeField] private AudioClip _correctProducts;
    [SerializeField] private AudioClip _incorrectProducts;

    private int _currentParentIndex;
    private int _wrongProducts;
    private bool _hasWrongProduct;

    private List<ProductBase> _products = new List<ProductBase>();

    [Space(10)]
    [SerializedDictionary("Order Name", "Amount")]
    public SerializedDictionary<string, int> _ordersAmount;

    private void Start()
    {
        _confirmBt.onClick.AddListener(ConfirmProducts);
        _declaineBt.onClick.AddListener(RemoveProductsFromBasket);
    }

    public void SetOrders(List<Order> orders)
    {
        _ordersAmount = new SerializedDictionary<string, int>();
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

        if (!_products.Contains(product))
        {
            _products.Add(product);
            DiscountProductAmount(product);
        }

        _currentParentIndex = (_currentParentIndex + 1) % _productsParents.Count;
    }

    private void DiscountProductAmount(ProductBase product)
    {
        if (_ordersAmount == null) return;

        if (_ordersAmount.ContainsKey(product.ProductName))
        {
            _ordersAmount[product.ProductName]--;
            if (_ordersAmount[product.ProductName] == 0)
            {
                _personOrderUI.DiscardProduct(product.ProductName);
            }
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
            AudioManager.OnPlaySFX(_correctProducts);
            LeanTween.moveY(gameObject, transform.position.y + 0.5f, 0.15f).setEaseInQuad().setLoopPingPong(2);
            Events.Instance.OnAddScore(500);
            Events.Instance.OnGameEnded();
            return;
        }

        AudioManager.OnPlaySFX(_incorrectProducts);

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
                if (_ordersAmount.ContainsKey(product.ProductName))
                {
                    if (_ordersAmount[product.ProductName] == 0) _personOrderUI.LetAvailable(product.ProductName);
                    _ordersAmount[product.ProductName]++;
                }

                product.RemoveFromBasket();
                _wrongProducts--;

                if (_wrongProducts == 0) _hasWrongProduct = false;

                if (_products.Contains(product)) _products.Remove(product);
            }
        }
        _currentParentIndex = 0;
    }

    private void RemoveProductWhenDragging(ProductLevel3 product)
    {
        if (_ordersAmount.ContainsKey(product.ProductName))
        {
            if (_ordersAmount[product.ProductName] == 0)
            {
                _personOrderUI.LetAvailable(product.ProductName);
            }
            _ordersAmount[product.ProductName]++;
        }
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

        if (_products.Contains(product)) _products.Remove(product);

        product.ClearBasketReference();
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
                RemoveProductWhenDragging(product);
            }
        }
    }

}
