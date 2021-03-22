using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridHandler : MonoBehaviour
{
    public Cell cell;

    [Tooltip("Steps per second")]
    public float snakeMoveDelay = 1f;

    [Tooltip("Snake won't move faster than this")]
    public float minimumMoveDelay = .1f;

    [Tooltip("The reduction of speed each apple eaten")]
    public float moveDelayDecrement = .05f;

    [Range(0.1f, 1)] public float scale;
    [SerializeField] private EndPanel endPanel;

    private DataCollector dataCollector;
    private GridLayoutGroup gridLayout;
    private Vector2 cellSize;
    private Cell[,] cells;
    private Snake snake;
    private float snakeMoveStart;
    private bool gameOver = false;
    private int collectedApples = 0;

    public int CollectedApples => collectedApples;

    public float StepDelay => snakeMoveDelay;

    public Vector2 GridSize { get; private set; }
    public Snake GetSnake() => snake;
    
    // Start is called before the first frame update
    void Awake()
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
        //debug
        if(!gameOver && Input.GetKeyDown(KeyCode.Q))
            GameOver();
    }

    private void FixedUpdate()
    {
        Vector2 prevPos = snake.head;

        if (!gameOver && snakeMoveDelay < Time.time - snakeMoveStart)
        {
            if (snake.Move())
                snakeMoveStart = Time.time;
            else
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        endPanel.gameObject.SetActive(true);
        gameOver = true;
    }

    private void Initialize()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        GridSize = AspectRatio.GetAspectRatio(Screen.width, Screen.height);
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

        dataCollector = new DataCollector();
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
        collectedApples++;

        // MoveDelay som börjar på 1 och minskar .05 varje äpple kändes lagom. Skulle dock vilja kunna minska storleken på spelplanen.
        snakeMoveDelay = Mathf.Max(minimumMoveDelay, snakeMoveDelay - moveDelayDecrement);

        PlaceApple();
    }

    public Cell GetCell(Vector2 pos) => cells[(int)pos.x, (int)pos.y];
}
