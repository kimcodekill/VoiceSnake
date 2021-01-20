using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHandler : MonoBehaviour
{
    public PlayerSide sidePrefab;
    public Image dividerPrefab;

    private PlayerSide player;
    
    private void Awake()
    {
        RectTransform r = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        float halfWidth = r.rect.width / 2f;
        float quarterWidth = halfWidth / 2f;
        float height = r.rect.height;
        float halfHeight = height / 2f;
        
        Vector2 sideSize = new Vector2(halfWidth, height);
        Vector2 p0AnchorMin = Vector2.zero;
        Vector2 p0AnchorMax = new Vector2(0.5f, 1f); 
        Vector2 p1AnchorMin = new Vector2(0.5f, 0f);
        Vector2 p1AnchorMax = new Vector2(1f,   1f);
            
        PlayerSide p0 = Instantiate(sidePrefab) as PlayerSide;
        p0.transform.parent = transform;
        p0.Initialize(0, p0AnchorMin, p0AnchorMax);

        PlayerSide p1 = Instantiate(sidePrefab) as PlayerSide;
        p1.transform.parent = transform;
        p1.Initialize(1, p1AnchorMin, p1AnchorMax);

        DrawDivider();
    }

    private void DrawDivider()
    {
        Image divider = Instantiate(dividerPrefab) as Image;
        divider.transform.SetParent(transform, false);
    }
}
