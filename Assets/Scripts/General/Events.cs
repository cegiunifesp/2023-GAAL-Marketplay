using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    private static Events _instance;
    public static Events Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Events>();
            return _instance;
        }
        set
        {
            if (_instance == null) _instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public  Action onGameStart { get; set; }
    public void OnGameStart()
    {
        onGameStart?.Invoke();
    }

    public Action onGameEnded { get; set; }
    public void OnGameEnded()
    {
        onGameEnded?.Invoke();
    }

    #region Level 1
    public Action<ProductLevel1> onEnqueueProduct { get; set; }
    public void OnEnqueueProduct(ProductLevel1 product)
    {
        onEnqueueProduct?.Invoke(product);
    }

    public Action<ProductLevel1> onProductSelected { get; set; }
    public void OnProductSelected(ProductLevel1 product)
    {
        onProductSelected?.Invoke(product);
    }
    #endregion

    #region Level 2
    public Action<int> onShelfCompleted { get; set; }
    public void OnShelfCompleted(int index)
    {
        onShelfCompleted?.Invoke(index);
    }
    #endregion

    public Action<bool> onPause { get; set; }
    public void OnPause(bool paused)
    {
        onPause?.Invoke(paused);
    }

    public Action<int> onAddScore { get; set; }
    public void OnAddScore(int amount)
    {
        onAddScore?.Invoke(amount);
    }
    public void OnRemoveScore(int amount)
    {
        onAddScore?.Invoke(-amount);
    }
}
