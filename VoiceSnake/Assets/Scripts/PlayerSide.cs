using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class PlayerSide : MonoBehaviour
{
    public ShipCell shipCellPrefab;
    public RectTransform legendPrefab;
    [Range(0.1f, 1f)]public float cellScale = 1f;
    
    private const int gridSize = 6;
    
    public Color Color { get; private set; }

    private RectTransform rect;
    private ShipCell[] cells;
    private int playerIndex = 0;
    private float width, height;
    private float cellSize;
    private Vector2 cellSizeRatio;
    private Vector2 scaledCellSizeRatio;
    private Vector2 padding;

    public int PlayerIndex => playerIndex; 

    private string[] legend =
    {
        // "J", "I", "H", "G",
        "F", "E", "D", "C", "B", "A",  
        "1", "2", "3", "4", "5", "6"
         //, "7", "8", "9", "10"
    };

    public void Initialize(int playerIndex, Vector2 anchorMin, Vector2 anchorMax)
    {
        this.playerIndex = playerIndex;
        Color = playerIndex == 0 ? Color.green : Color.red;
        rect = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMax = rect.offsetMin = Vector2.zero;
    }
    
    private void Start()
    {
        cells = new ShipCell[gridSize * gridSize];
        width = rect.rect.width;
        height = rect.rect.height;
        cellSize = width / gridSize;
        cellSizeRatio = new Vector2(cellSize / width, cellSize / height);
        padding = cellSizeRatio * 0.1f;
        scaledCellSizeRatio = cellSizeRatio * cellScale;

        DrawLegend(new Vector2(padding.x , 0));
        PopulateShipCells(new Vector2(scaledCellSizeRatio.x + padding.x * 2f, 0f));
        
        print($"Player {playerIndex} is ready");
    }

    private void DrawLegend(Vector2 origin)
    {
        Vector2 cellAnchorMin = Vector2.zero;
        Vector2 cellAnchorMax = Vector2.zero; 
        
        for (int y = 0, i = 0; y <= gridSize; y++)
        {
            int rowWidth = y == gridSize ? gridSize : 1;
            float shift = rowWidth == gridSize ? scaledCellSizeRatio.x + padding.x : 0f;
            float offsetY = origin.y + padding.y * (y + 1);
            cellAnchorMin.y = y * scaledCellSizeRatio.y + offsetY;
            cellAnchorMax.y = (y + 1) * scaledCellSizeRatio.y + offsetY;

            for (int x = 0; x < rowWidth; x++, i++)
            {
                RectTransform cell = Instantiate(legendPrefab) as RectTransform;
                cell.transform.parent = transform;
                
                float xOffset = shift + origin.x + padding.x * (x + 1);
                cellAnchorMin.x = x * scaledCellSizeRatio.x + xOffset;
                cellAnchorMax.x = (x + 1) * scaledCellSizeRatio.x + xOffset;
                cell.anchorMin = cellAnchorMin;
                cell.anchorMax = cellAnchorMax;
                cell.offsetMax = cell.offsetMin = Vector2.zero;
                
                Text txt = cell.GetComponentInChildren<Text>();
                txt.text = legend[i];
                txt.fontSize = (int) (cellSize * 0.5f);
            }
        }
    }

    private void PopulateShipCells(Vector2 origin)
    {
        Vector2 cellAnchorMin = Vector2.zero;
        Vector2 cellAnchorMax = Vector2.zero;
        float yMax = origin.y + gridSize * scaledCellSizeRatio.y + gridSize * padding.y;
        
        for (int y = 0, i = 0; y < gridSize; y++)
        {
            float offsetY = padding.y * (y);
            
            cellAnchorMax.y = yMax - (y * scaledCellSizeRatio.y) - offsetY;
            cellAnchorMin.y = yMax - ((y + 1) * scaledCellSizeRatio.y) - offsetY;
            
            for (int x = 0; x < gridSize; x++, i++)
            {
                ShipCell shipCell = Instantiate(shipCellPrefab) as ShipCell;
                shipCell.transform.parent = transform;
                shipCell.transform.name = $"Cell {i} ({x},{y})";
                shipCell.Initialize(new Vector2(x, y), this);

                float offsetX = origin.x + padding.x * (x + 1);

                cellAnchorMin.x = x * scaledCellSizeRatio.x + offsetX;
                cellAnchorMax.x = (x + 1) * scaledCellSizeRatio.x + offsetX;

                RectTransform cRect = shipCell.GetComponent<RectTransform>();
                cRect.anchorMin = cellAnchorMin;
                cRect.anchorMax = cellAnchorMax;
                cRect.offsetMax = cRect.offsetMin = Vector2.zero;
                
                cells[i] = shipCell;
            }
        }
    }

    public void SelectCell(SelectType selectType, int x, int y)
    {
        cells[x + y * (gridSize - 1)].Select(selectType);
    }

    public Rotation[] AllowedRotations(Vector2 gridPos)
    {
        return new Rotation[] {Rotation.North, Rotation.East, Rotation.South, Rotation.West};
    }
}
