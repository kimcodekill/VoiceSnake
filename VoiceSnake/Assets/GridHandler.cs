using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridHandler : MonoBehaviour
{
    public Cell cell;
    public float snakeMoveDelay = 1f;
    [Range(1, 10)] public int scale;
    
    private GridLayoutGroup gridLayout;
    private Vector2 cellSize;
    private Cell[,] cells;
    private Snake snake;
    private float snakeMoveStart;
    private bool gameOver = false;
    
    public Vector2 GridSize { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        snake = new Snake(new Vector2(GridSize.x / 2, GridSize.y / 2), this);
        Cell snakeHead = GetCell(snake.head);
        snakeHead.State = CellState.Snake;
        snake.MoveDir = Vector2.zero;

        Cell apple = PlaceApple();
    }

    private void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        if (hor != 0)
            snake.MoveDir = Vector2.right * hor;
        else if (vert != 0)
            snake.MoveDir = Vector2.down * vert;
    }

    private void FixedUpdate()
    {
        Vector2 prevPos = snake.head;

        if (!gameOver && snakeMoveDelay < Time.time - snakeMoveStart)
        {
            if (snake.Move())
                snakeMoveStart = Time.time;
            else
                gameOver = true;
        }
    }

    private void Initialize()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        GridSize = AspectRatio.GetAspectRatio(Screen.width, Screen.height) * scale;
        cells = new Cell[(int)GridSize.x, (int) GridSize.y];
        
        Canvas.ForceUpdateCanvases();
        RectTransform pr = transform.parent.GetComponent<RectTransform>();
        cellSize = new Vector2(pr.rect.width, pr.rect.height);
        cellSize.x /= GridSize.x;
        cellSize.y /= GridSize.y;
        gridLayout.cellSize = cellSize * 0.9f;
        Vector2 padding = cellSize * 0.05f;

        gridLayout.padding = new RectOffset((int)padding.x, (int)padding.x, (int)padding.y, (int)padding.y);
        gridLayout.spacing = padding * 2f;
        //gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;
        //gridLayout.constraintCount = (int) gridSize.x;

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                cells[x, y] = Instantiate(cell, transform) as Cell;
                cells[x, y].gameObject.name = $"Cell ({x}, {y})";
            }
        }
            
    }

    private Cell PlaceApple()
    {
        int x = Random.Range(0, (int) GridSize.x);
        int y = Random.Range(0, (int) GridSize.y);
        Cell appleCandidate = cells[x,y];
        if (appleCandidate.State == CellState.Empty)
        {
            appleCandidate.State = CellState.Apple;
            return appleCandidate;
        }
        else
            return PlaceApple();
    }

    public void EatApple()
    {
        snakeMoveDelay = Mathf.Max(0f, snakeMoveDelay - Time.deltaTime); //temp
        PlaceApple();
    }

    public Cell GetCell(Vector2 pos) => cells[(int)pos.x, (int)pos.y];
}
