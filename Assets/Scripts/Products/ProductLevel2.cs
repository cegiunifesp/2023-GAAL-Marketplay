using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ProductLevel2 : ProductBase, IPointerEnterHandler, IPointerDownHandler
{
    private Enums.StatesDrag _state;

    [SerializeField] private float _speedMovement;
    [SerializeField] private Transform _canvas;

    private int _childIndex;

    private Camera _cam;
    private Shelf _shelf;
    private HorizontalLayoutGroup _initialParent;


    private void Update()
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.OnDrag:
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
        _initialParent = transform.parent.GetComponent<HorizontalLayoutGroup>();
        _canvas = GameObject.Find("Canvas").transform;
        _cam = Camera.main;

        Size = _initialParent.GetComponent<RectTransform>().rect.height;
        _defaultSize = Size;

        Interactable = true;

        base.InitiateProduct(info);
    }

    private void InitialParent(bool removedFromShelf = false)
    {
        Size = _defaultSize;
        AdjustImage();

        Vector3 oldPosition = transform.position;
        Vector3 newPosition;

        if (_initialParent.transform.childCount == 0) newPosition = _initialParent.transform.position;
        else
        {
            if (removedFromShelf)
            {
                newPosition = _initialParent.transform.GetChild(_initialParent.transform.childCount - 1).position;
                _childIndex = _initialParent.transform.childCount;
            }
            else
            {
                int index = _childIndex == _initialParent.transform.childCount ? _childIndex - 1 : _childIndex;
                newPosition = _initialParent.transform.GetChild(index).position;
            }
        }

        transform.position = oldPosition;
        transform.SetParent(_canvas);

        LeanTween.move(gameObject, newPosition, 1).setEaseOutQuad().setOnComplete(() =>
        {
            if (_state == Enums.StatesDrag.OnDrag) return;

            transform.SetParent(_initialParent.transform);
            transform.SetSiblingIndex(_childIndex);
        });
    }

    public void SetInteractable(bool interactable)
    {
        Interactable = interactable;
    }

    #region Events
    private void OnPointerUp()
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.OnDrag:
                if (_shelf == null)
                {
                    _state = Enums.StatesDrag.Initial;
                    InitialParent();
                }
                else
                {
                    if (_shelf.AddProduct(this))
                    {
                        _state = Enums.StatesDrag.OnSlot;
                    }
                    else
                    {
                        _state = Enums.StatesDrag.Initial;
                        InitialParent();
                        _shelf = null;
                    }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;

        switch (_state)
        {
            case Enums.StatesDrag.Initial:
                _state = Enums.StatesDrag.OnDrag;
                _childIndex = transform.GetSiblingIndex();
                transform.SetParent(_canvas);
                break;
            case Enums.StatesDrag.OnSlot:
                _state = Enums.StatesDrag.OnDrag;
                transform.SetParent(_canvas);
                AdjustImage();
                Replacing();
                break;
            default:
                break;
        }
    }
    #endregion

    #region Shelf Related
    public void SetShelfReferenceWhenDragging(Shelf shelf)
    {
        if (_state == Enums.StatesDrag.OnDrag)
        {
            _shelf = shelf;
        }
    }

    public void ClearShelfReferenceWhenDragging(Shelf shelf)
    {
        if (_state == Enums.StatesDrag.OnDrag)
        {
            if (_shelf == shelf) _shelf = null;
        }
    }

    public void AddedToShelf(float size)
    {
        Size = size;
        AdjustImage();
        Size = _defaultSize;
    }

    private void Replacing()
    {
        if (_shelf != null)
        {
            _shelf.ProductReplaced(ProductName);
        }
    }

    public void ShelfRemovedProduct()
    {
        if (_shelf != null)
        {
            //_shelf.ProductReplaced(ProductName);
            _shelf = null;
        }
        else
        {
            Debug.LogError("Slot is null");
        }

        InitialParent(true);
    }
    #endregion
}
