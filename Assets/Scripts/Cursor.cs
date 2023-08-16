using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cursor : MonoBehaviour
{
    [SerializeField] private float _swingIntensity;

    private bool _stopWorking;
    private Vector3 _lastMousePosition;

    private Camera _cam;
    private GameObject _lastObjectSelected;

    private void Start()
    {
        _cam = Camera.main;

        Events.Instance.onPause += HandlePause;
        Events.Instance.onGameEnded += HandleGameEnded;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopWorking) return;

        Vector3 currentMousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousedelta = currentMousePosition - _lastMousePosition;

        float rotationAmount = -mousedelta.x * _swingIntensity;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAmount);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        _lastMousePosition = currentMousePosition;


        Vector2 newPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newPosition;

        if (Input.GetMouseButtonDown(0))
        { 
            if (_lastObjectSelected != null)
            {
                if (_lastObjectSelected.TryGetComponent(out ProductLevel1 product))
                {
                    if (product != null) Events.Instance.OnProductSelected(product);
                }
                _lastObjectSelected = null;
            }
        }

    }

    private void HandlePause(bool paused)
    {
        _stopWorking = paused;
    }

    private void HandleGameEnded()
    {
        _stopWorking = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            _lastObjectSelected = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject objectCollided = collision.gameObject;
        if (objectCollided.CompareTag("Product"))
        {
            if (_lastObjectSelected == objectCollided) _lastObjectSelected = null;
        }
    }
}
