using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public PlayerSide sidePrefab;
    public Image dividerPrefab;
    public Action OnClick;
    
    private static int currPlayer;
    private static GameState gameState;
    private static Cell selectedCell;
    
    private PlayerSide[] sides;
    
    private float halfWidth;
    private float quarterWidth;
    private float height;
    private float halfHeight;
    
    public static GameState CurrentState => gameState;
    public static int CurrentPlayer => currPlayer;

    public static Cell SelectedCell
    {
        get => selectedCell;
        set
        {
            if(selectedCell != null) selectedCell.Select(SelectType.None);
            selectedCell = value;
        }
    }

    public static void FinishTurn(int playerIndex)
    {
        currPlayer = playerIndex == 0 ? 1 : 0;
        //do whatever else
    }
    
    private void Awake()
    {
        sides = new PlayerSide[2];
        
        RectTransform r = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        halfWidth = r.rect.width / 2f;
        quarterWidth = halfWidth / 2f;
        height = r.rect.height;
        halfHeight = height / 2f;
        
        CreateSides();

        DrawDivider();
        currPlayer = 0;
        gameState = GameState.Setup;
    }

    private void CreateSides()
    {
        Vector2 sideSize = new Vector2(halfWidth, height);
        Vector2 p0AnchorMin = Vector2.zero;
        Vector2 p0AnchorMax = new Vector2(.5f, 1f);
        Vector2 p1AnchorMin = new Vector2(.5f, 0f);
        Vector2 p1AnchorMax = new Vector2(1f, 1f);

        PlayerSide p0 = Instantiate(sidePrefab) as PlayerSide;
        p0.transform.parent = transform;
        p0.Initialize(0, p0AnchorMin, p0AnchorMax);
        sides[0] = p0;

        PlayerSide p1 = Instantiate(sidePrefab) as PlayerSide;
        p1.transform.parent = transform;
        p1.Initialize(1, p1AnchorMin, p1AnchorMax);
        sides[1] = p1;
    }

    private void DrawDivider()
    {
        Image divider = Instantiate(dividerPrefab) as Image;
        divider.transform.SetParent(transform, false);
    }
}
