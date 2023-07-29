using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Events.onGameStart += HandleStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleStart()
    {

    }
}
