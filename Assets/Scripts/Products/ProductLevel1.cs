using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ProductLevel1 : ProductBase
{
    private const float SpeedOverTreadmill = 1;

    private bool _move;
    public VideoClip Clip => InfoFromSO.Clip;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject _check;


    private void Start()
    {
        Events.Instance.onGameEnded += HandleGameEnded;
    }


    private void Update()
    {
        if (_move)
        {
            transform.Translate(Vector3.right * Time.deltaTime * SpeedOverTreadmill);
        }
    }


    public override void InitiateProduct(ProductSO info)
    {
        base.InitiateProduct(info);

        _move = true;
        Interactable = true;
        _check.SetActive(false);
    }

    protected override void AdjustImage()
    {
        base.AdjustImage();

        var size = ImageSource.rectTransform.sizeDelta;
        AdjustPositionOverTreadmill(size);
    }

    private void AdjustPositionOverTreadmill(Vector2 size)
    {
        Vector3 newPosition = Vector3.zero;
        if (size.y / 2 > 50) newPosition = Vector3.up * size.y / 2.75f;
        else if (size.y / 2 < 50 && size.y / 2 > 25) newPosition = Vector3.up * size.y / 7f;

        transform.localPosition = newPosition;
    }

    public void HandleGameEnded()
    {
        _move = false;
    }

    public void ProductIncorrect()
    {
        transform.eulerAngles = Vector3.forward * 10;
        LeanTween.rotateZ(gameObject, -10, 0.2f).setLoopPingPong(1).setOnComplete(() =>
        {
            LeanTween.rotateZ(gameObject, 0, 0.2f);
        });

        _rigidbody.AddForce(Vector2.up * Random.Range(3, 7), ForceMode2D.Impulse);
        BoxCollider.isTrigger = true;
        Interactable = false;
    }

    public void ProductCorrect()
    {
        _check.SetActive(true);
        Interactable = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.Instance.OnEnqueueProduct(this);
            _move = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.Instance.OnEnqueueProduct(this);
            BoxCollider.isTrigger = false;
            _move = false;
        }
    }
}
