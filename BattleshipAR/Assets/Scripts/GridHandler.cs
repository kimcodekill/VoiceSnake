using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHandler : MonoBehaviour
{
    public PlayerSide sidePrefab;

    private PlayerSide player;
    
    private void Awake()
    {
        
        PlayerSide p0 = Instantiate(sidePrefab) as PlayerSide;
        p0.transform.parent = transform;
        p0.Initialize(0);
        
        PlayerSide p1 = Instantiate(sidePrefab) as PlayerSide;
        p1.transform.parent = transform;
        p1.Initialize(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
