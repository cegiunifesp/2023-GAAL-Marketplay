using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Product : MonoBehaviour
{
    private const float ConstSize = 125;
    private const float SpeedOverTreadmill = 1;

    [SerializeField] private ProductSO _infoFromSO;
    [SerializeField] private Image _imageSource;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Rigidbody2D _rigidbody;

    public string ProductName => _infoFromSO.ProductName;
    public VideoClip Clip => _infoFromSO.Clip;


    private bool _move;

    private void Start()
    {
        Events.onGameEnded += HandleGameEnded;
    }


    private void Update()
    {
        if (_move)
        {
            transform.Translate(Vector3.right * Time.deltaTime * SpeedOverTreadmill);
        }
    }

    public void InitiateProduct(ProductSO info)
    {
        _infoFromSO = info;
        _imageSource.sprite = _infoFromSO.SpriteSource;
        _imageSource.SetNativeSize();

        var size = _imageSource.rectTransform.sizeDelta;
        AdjustSize(size);

        size = _imageSource.rectTransform.sizeDelta;
        AdjustPositionOverTreadmill(size);

        _boxCollider.size = size;

        _move = true;
    }

    private void AdjustPositionOverTreadmill(Vector2 size)
    {
        Vector3 newPosition = Vector3.zero;
        if (size.y / 2 > 50) newPosition = Vector3.up * size.y / 2.75f;
        else if (size.y / 2 < 50 && size.y / 2 > 25) newPosition = Vector3.up * size.y / 7f;

        transform.localPosition = newPosition;
    }

    private void AdjustSize(Vector2 size)
    {
        float proportion = size.x / size.y;

        if (_infoFromSO.AdjustHorizontally) _imageSource.rectTransform.sizeDelta = new Vector2(ConstSize * proportion, ConstSize);
        else _imageSource.rectTransform.sizeDelta = new Vector2(ConstSize, ConstSize/ proportion);
    }

    public void HandleGameEnded()
    {
        _move = false;
    }


    public void ProductIncorrect()
    {
        _rigidbody.AddForce(Vector2.up * Random.Range(3, 7), ForceMode2D.Impulse);
        _boxCollider.isTrigger = true;
    }

    public void ProductCorrect()
    {
        _boxCollider.isTrigger = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.OnEnqueueProduct(this);
            _move = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.OnEnqueueProduct(this);
            _boxCollider.isTrigger = false;
            _move = false;
        }
    }
}
