using UnityEngine.UI;
using UnityEngine;

public class PersonOrderUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _people;
    [SerializeField] private Sprite[] _numbersSprite;

    [Space(10)]
    [SerializeField] private Image _personImgSource;
    [SerializeField] private Transform _ordersParent;
    [SerializeField] private OrderUI _orderPrefab;

    public void Initiate(Order[] orders)
    {
        _personImgSource.sprite = _people.GetRandomValue();

        LeanTween.scale(gameObject, Vector2.zero, 0.01f).setOnComplete(() =>
        {
            LeanTween.scale(gameObject, Vector3.one, 0.75f).setEaseOutQuart();
        }) ;


        InstantiateOrders(orders);
    }

    private void InstantiateOrders(Order[] orders)
    {
        foreach (Order order in orders)
        {
            var newOrder = Instantiate(_orderPrefab, _ordersParent);
            newOrder.Initiate(_numbersSprite[order.ProductAmount], order.ProductSprite);
        }
    }
}
