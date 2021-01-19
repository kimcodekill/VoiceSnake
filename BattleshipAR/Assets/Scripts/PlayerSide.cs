using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class PlayerSide : MonoBehaviour
{
    public Image cellPrefab;
    public RectTransform legendPrefab;
    
    private const int gridSize = 10;

    private RectTransform rect;
    private CellState[] cells;
    private int playerIndex = 0;
    private float size;
    private float cellSize;
    private float padding;

    private string[] legend =
    {
        "J", "I", "H", "G", "F", "E", "D", "C", "B", "A",  
        "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"
    };
    

    public void Initialize(int playerIndex, Vector2 anchorMin, Vector2 anchorMax)
    {
        this.playerIndex = playerIndex;
        rect = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMax = rect.offsetMin = Vector2.zero;
    }
    
    private void Start()
    {
        cells = new CellState[gridSize * gridSize];
        size = rect.rect.width;
        cellSize = size / gridSize;
        padding = cellSize * 3; //size * 0.05f;

        //DrawLegend();
        PopulateShipCells();
        
        print($"Player {playerIndex} is ready");
    }

    private void DrawLegend()
    {
        for (int y = 0, i = 0; y <= gridSize; y++)
        {
            int rowWidth = y == gridSize ? gridSize : 1;
            float offset = rowWidth == gridSize ? cellSize : 0f;
            for (int x = 0; x < rowWidth; x++, i++)
            {
                RectTransform legendCell = Instantiate(legendPrefab) as RectTransform;
                legendCell.transform.parent = transform;
                Vector2 offsetPos = new Vector2(offset + padding * 0.66f, padding / 6);
                Vector2 cellPos = new Vector2(x * cellSize, y * cellSize) + offsetPos;
                legendCell.anchoredPosition = cellPos;
                legendCell.sizeDelta = Vector2.one * cellSize * 0.9f;
                
                Text txt = legendCell.GetComponentInChildren<Text>();
                txt.text = legend[i];
                txt.fontSize = (int) (cellSize * 0.8f);
            }
        }
    }

    private void PopulateShipCells()
    {
        float cellSizeRatio = cellSize / size;
        float paddingRatio = cellSizeRatio * 0.25f;
        Vector2 cellAnchorMin = Vector2.zero;
        Vector2 cellAnchorMax = Vector2.zero;
        float offsetX = cellSizeRatio + paddingRatio;
        float offsetY = paddingRatio;
        
        for (int y = 0, i = 0; y < gridSize; y++)
        {
            cellAnchorMin.y = y * cellSizeRatio;
            cellAnchorMax.y = (y + 1) * cellSizeRatio;
            for (int x = 1; x <= gridSize; x++, i++)
            {
                
                Image img = Instantiate(cellPrefab) as Image;
                img.color = playerIndex == 0 ? Color.green : Color.red;
                
                img.transform.parent = transform;
                
                cellAnchorMin.x = x * cellSizeRatio;
                cellAnchorMax.x = (x + 1) * cellSizeRatio;
                
                img.rectTransform.anchorMin = cellAnchorMin;
                img.rectTransform.anchorMax = cellAnchorMax;
                img.rectTransform.offsetMax = img.rectTransform.offsetMin = Vector2.zero;
                
                cells[i] = CellState.Empty;
            }
        }
    }
}
