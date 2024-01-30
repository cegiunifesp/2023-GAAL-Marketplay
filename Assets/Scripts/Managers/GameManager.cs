using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load("Prefabs/Game Manager")).GetComponent<GameManager>();
            }
            return _instance;
        }
        set
        {
            if (_instance == null) _instance = value;
        }
    }
    [field: SerializeField] public bool UrlVideo { get; private set; }

    [SerializeField] private List<ProductSO> _breakfastProducts;
    [SerializeField] private List<ProductSO> _lunchProducts;
    [SerializeField] private List<ProductSO> _hygieneProducts;

    public Dictionary<Enums.TypeProducts, List<ProductSO>> ListProducts { get; private set; } = new Dictionary<Enums.TypeProducts, List<ProductSO>>();

    public Enums.TypeProducts TypeSelected { get; private set; }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;

        ListProducts.Add(Enums.TypeProducts.Cafe, _breakfastProducts);
        ListProducts.Add(Enums.TypeProducts.Almoco, _lunchProducts);
        ListProducts.Add(Enums.TypeProducts.Higiene, _hygieneProducts);
    }

    public List<ProductSO> GetProductsAvailables()
    {
        if (TypeSelected == Enums.TypeProducts.None) SetTypeSelected(0);

        var newList = new List<ProductSO>(ListProducts[TypeSelected]);

        return newList;
    }

    public void SetTypeSelected(int index)
    {
        if (index <= 0)
        {
            TypeSelected = (Enums.TypeProducts)Random.Range(1, 4);
        }
        else
        {
            TypeSelected = (Enums.TypeProducts)index;
        }
    }
}
