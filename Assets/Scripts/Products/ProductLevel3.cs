using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class ProductLevel3 : ProductBase, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler
{
    private Enums.StatesDrag _state;

    [SerializeField] private Transform _intermediateParent;
    [SerializeField] private float _speedMovement;

    private int _index;

    private bool _leftBorder = true;

    private Vector3 _initialPosition;
    private Vector3 _initialRotation;

    private Transform _initialParent;

    private Camera _cam;
    private Basket _basket;


    private void Update()
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.OnDrag:
                //print($"Moving {gameObject.name}");
                Vector3 mouse = _cam.ScreenToWorldPoint(Input.mousePosition);
                mouse.z = 0;
                var dir = mouse - transform.position;
                transform.Translate(dir * Time.deltaTime * _speedMovement);
                if (Input.GetMouseButtonUp(0))
                {
                    OnPointerUp();
                }
                break;
            default:
                break;
        }
    }

    public override void InitiateProduct(ProductSO info)
    {
        _index = transform.GetSiblingIndex();
        _initialParent = transform.parent;
        _initialPosition = transform.position;
        _initialRotation = transform.rotation.eulerAngles;

        _cam = Camera.main;

        Size = GetComponent<RectTransform>().rect.height;
        _defaultSize = Size;

        Interactable = true;

        base.InitiateProduct(info);

        Events.Instance.onGameEnded += HandleEndGame;
    }

    private void HandleEndGame()
    {
        Interactable = false;
    }

    public void MakeItInteractable()
    {
        _index = transform.GetSiblingIndex();
        _initialParent = transform.parent;
        _initialPosition = transform.position;
        _initialRotation = transform.rotation.eulerAngles;

        _cam = Camera.main;

        Size = GetComponent<RectTransform>().rect.height;
        _defaultSize = Size;

        Interactable = true;
    }

    private void InitialParent()
    {
        Size = _defaultSize;
        AdjustImage();

        if (_leftBorder)
        {
            LeanTween.move(gameObject, _initialPosition, 1.5f).setEaseOutQuad().setOnComplete(() =>
            {
                if (_state == Enums.StatesDrag.OnDrag) return;

                transform.SetParent(_initialParent);
                transform.SetSiblingIndex(_index);
                transform.eulerAngles = _initialRotation;
            });
        }
        else
        {
            transform.SetParent(_initialParent);
            transform.eulerAngles = _initialRotation;
            _initialPosition = transform.position;
        }

    }

    internal bool IsDragging()
    {
        return _state == Enums.StatesDrag.OnDrag;
    }


    #region Events
    private void OnPointerUp()
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.OnDrag:
                if (_basket == null)
                {
                    _state = Enums.StatesDrag.Initial;
                    InitialParent();
                }
                else
                {
                    _state = Enums.StatesDrag.OnSlot;
                    _basket.AddProduct(this);
                    AddedToBasket();
                }
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.Initial:
            case Enums.StatesDrag.OnSlot:
                break;
            default:
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.Initial:
                _state = Enums.StatesDrag.OnDrag;
                transform.SetParent(_intermediateParent);
                break;
            case Enums.StatesDrag.OnSlot:
                _state = Enums.StatesDrag.OnDrag;
                transform.SetParent(_intermediateParent);
                AdjustImage();
                break;
            default:
                break;
        }
    }
    #endregion


    #region Basket
    private void AddedToBasket()
    {
        AdjustImage();
        Size = _defaultSize;
    }

    public void RemoveFromBasket()
    {
        if (_basket != null)
        {
            _basket = null;
        }
        else Debug.LogError("Slot is null");

        InitialParent();
    }

    public void SetBasket(Basket basket)
    {
        _basket = basket;
    }

    public void ClearBasketReference()
    {
        if (_state == Enums.StatesDrag.OnDrag)
        {
            //print("Left Basket");
            _basket = null;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border")) _leftBorder = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border")) _leftBorder = true;
    }
}
