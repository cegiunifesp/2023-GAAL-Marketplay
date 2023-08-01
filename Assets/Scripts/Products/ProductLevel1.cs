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
        Events.onGameEnded += HandleGameEnded;
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

    public void HandleGameEnded()
    {
        _move = false;
    }

    public void ProductIncorrect()
    {
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
            Events.OnEnqueueProduct(this);
            _move = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Limit"))
        {
            Events.OnEnqueueProduct(this);
            BoxCollider.isTrigger = false;
            _move = false;
        }
    }
}
