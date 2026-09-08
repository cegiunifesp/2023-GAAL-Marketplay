using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basket : MonoBehaviour
{
    [SerializeField] private List<Transform> _productsParents;
    [SerializeField] private PersonOrderUI _personOrderUI;
    [SerializeField] private Button _confirmBt;
    [SerializeField] private Button _declaineBt;

    [SerializeField] private AudioClip _correctProducts;
    [SerializeField] private AudioClip _incorrectProducts;

    private int _currentParentIndex;
    private readonly List<ProductBase> _products = new List<ProductBase>();
    private readonly Dictionary<string, int> _requiredAmounts = new Dictionary<string, int>();
    private readonly Dictionary<string, int> _remainingAmounts = new Dictionary<string, int>();

    private void Start()
    {
        _confirmBt.onClick.AddListener(ConfirmProducts);
        _declaineBt.onClick.AddListener(RemoveProductsFromBasket);
    }

    public void SetOrders(List<Order> orders)
    {
        _requiredAmounts.Clear();
        _remainingAmounts.Clear();

        foreach (Order order in orders)
        {
            _requiredAmounts[order.ProductName] = order.ProductAmount;
            _remainingAmounts[order.ProductName] = order.ProductAmount;
        }

        RefreshOrderProgress();
    }

    public void AddProduct(ProductBase product)
    {
        Transform t = product.transform;
        t.SetParent(_productsParents[_currentParentIndex]);
        t.localPosition = Vector3.zero;
        product.Size = 80;

        if (!_products.Contains(product))
        {
            _products.Add(product);
        }

        _currentParentIndex = (_currentParentIndex + 1) % _productsParents.Count;
        RefreshOrderProgress();
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
        RefreshOrderProgress();

        foreach (var remaining in _remainingAmounts)
        {
            if (remaining.Value != 0)
            {
                RemoveProductsFromBasket();
                Events.Instance.OnRemoveScore(30);
                return false;
            }
        }

        int extraItems = _products.Count - GetRequiredTotal();
        if (extraItems != 0)
        {
            RemoveProductsFromBasket();
            Events.Instance.OnRemoveScore(30);
            return false;
        }

        return true;
    }

    private void RemoveProductsFromBasket()
    {
        var productsToReturn = new List<ProductLevel3>();
        for (int i = 0; i < _products.Count; i++)
        {
            if (_products[i] is ProductLevel3 product)
            {
                productsToReturn.Add(product);
            }
        }

        _products.Clear();
        _currentParentIndex = 0;

        for (int i = 0; i < productsToReturn.Count; i++)
        {
            productsToReturn[i].RemoveFromBasket();
        }

        RefreshOrderProgress();
    }

    private void RefreshOrderProgress()
    {
        if (_requiredAmounts.Count == 0) return;

        foreach (var required in _requiredAmounts)
        {
            _remainingAmounts[required.Key] = required.Value;
        }

        for (int i = 0; i < _products.Count; i++)
        {
            ProductBase product = _products[i];
            if (product == null) continue;

            if (_remainingAmounts.ContainsKey(product.ProductName))
            {
                _remainingAmounts[product.ProductName]--;
            }
        }

        foreach (var remaining in _remainingAmounts)
        {
            if (remaining.Value == 0)
            {
                _personOrderUI.DiscardProduct(remaining.Key);
            }
            else
            {
                _personOrderUI.LetAvailable(remaining.Key);
            }
        }
    }

    private int GetRequiredTotal()
    {
        int total = 0;
        foreach (var required in _requiredAmounts)
        {
            total += required.Value;
        }
        return total;
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
        if (!collision.gameObject.CompareTag("Product")) return;

        var product = collision.gameObject.GetComponent<ProductLevel3>();
        if (product == null || !product.IsDragging()) return;

        if (_products.Contains(product))
        {
            _products.Remove(product);
            RefreshOrderProgress();
        }

        product.ClearBasketReference();
    }
}
