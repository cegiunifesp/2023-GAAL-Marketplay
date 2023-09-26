using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManagerBase : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected VictoryScene VictoryScene;

    protected bool _gameStarted;
    protected bool _gameEnded;

    protected List<ProductSO> ProductsAvailables;
    protected List<ProductSO> OrdersAvailables;


    protected abstract void HandleStartGame();
    protected abstract void HandlePause(bool paused);
    protected abstract void HandleEndGame();
}
