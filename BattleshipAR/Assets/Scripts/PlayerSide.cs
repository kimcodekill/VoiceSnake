using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSide : MonoBehaviour
{
    public Image cellPrefab;
    public float padding = 3f;
    
    private const int gridSize = 10;

    private RectTransform rect;
    private bool[] cells;
    private int playerIndex = 0;
    private float size;
    private float cellSize;
    

    public void Initialize(int playerIndex)
    {
        this.playerIndex = playerIndex;
        rect = GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        cells = new bool[gridSize * gridSize];
        size = rect.rect.width * 0.9f;
        cellSize = size / gridSize;
        
        for (int y = 1, i = 0; y <= gridSize; y++)
        {
            for (int x = 1; x <= gridSize; x++, i++)
            {
                Image img = Instantiate(cellPrefab) as Image;
                img.transform.parent = transform;
                img.rectTransform.anchoredPosition = new Vector2(x * cellSize, y * cellSize);
                img.rectTransform.sizeDelta = new Vector2(cellSize * 0.9f, cellSize * 0.9f);
                img.color = playerIndex == 0 ? Color.green : Color.red;
                cells[i] = false;
            }
        }
    }
}
