using System;
using System.Collections;
using System.Collections.Generic;

public static class Events
{
    public static Action onGameStart { get; set; }
    public static void OnGameStart()
    {
        onGameStart?.Invoke();
    }

    public static Action onGameEnded { get; set; }
    public static void OnGameEnded()
    {
        onGameEnded?.Invoke();
    }

    public static Action<Product> onEnqueueProduct { get; set; }
    public static void OnEnqueueProduct(Product product)
    {
        onEnqueueProduct?.Invoke(product);
    }

    public static Action<Product> onProductSelected { get; set; }
    public static void OnProductSelected(Product product)
    {
        onProductSelected?.Invoke(product);
    }

    public static Action<bool> onPause { get; set; }
    public static void OnPause(bool paused)
    {
        onPause?.Invoke(paused);
    }

    public static Action<int> onAddScore { get; set; }
    public static void OnAddScore(int amount)
    {
        onAddScore?.Invoke(amount);
    }
    public static void OnRemoveScore(int amount)
    {
        onAddScore.Invoke(-amount);
    }
}
