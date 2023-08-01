using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ProductLevel2 : ProductBase, IPointerEnterHandler, IPointerDownHandler
{
    private Enums.StatesDrag _state;

    private HorizontalLayoutGroup _initialParent;
    [SerializeField] private Transform _canvas;

    private Camera _cam;
    private Shelf _shelf;

    [SerializeField] private float _speedMovement;

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

    private void InitialParent()
    {
        Size = _defaultSize;
        AdjustImage();

        Vector3 oldPosition = transform.position;
        Vector3 newPosition = _initialParent.transform.LastChildPosition() + Vector3.right;

        transform.position = oldPosition;
        LeanTween.move(gameObject, newPosition, 1f).setEaseOutQuad().setOnComplete(() =>
        {
            if (_state == Enums.StatesDrag.OnDrag) return;

            transform.SetParent(_initialParent.transform);
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
                    _shelf.AddProduct(this);
                    _state = Enums.StatesDrag.OnSlot;
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
                transform.SetParent(_canvas);
                break;
            case Enums.StatesDrag.OnSlot:
                _state = Enums.StatesDrag.OnDrag;
                transform.SetParent(_canvas);
                AdjustImage();
                break;
            default:
                break;
        }
    }

    public void ResetState()
    {
        if (_state == Enums.StatesDrag.OnSlot)
        {
            RemoveFromShelf();
        }
        _state = Enums.StatesDrag.Initial;
    }
    #endregion

    #region Shelf Related
    public void SetShelf(Shelf shelf)
    {
        _shelf = shelf;
    }

    public void ClearShelf(Shelf shelf)
    {
        if (_shelf == shelf) _shelf = null;
    }

    public void AddedToShelf(float size)
    {
        Size = size;
        AdjustImage();
        Size = _defaultSize;
    }

    public void RemoveFromShelf()
    {
        if (_shelf != null)
        {
            _shelf.RemovedProduct();
            _shelf = null;
            InitialParent();
        }
        else
        {
            Debug.LogError("Slot is null");
        }
    }
    #endregion
}
