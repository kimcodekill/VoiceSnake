using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Color emptyColor;
    [FormerlySerializedAs("snakeColor")] public Color snakeBodyColor;
    public Color snakeHeadColor;
    public Color appleColor;

    private Image image;
    
    private CellState state;
    public CellState State
    {
        get => state;
        set
        {
            state = value;
            UpdateColor();
        }
    }

    void Awake()
    {
        image = GetComponent<Image>();
    }
    
    private void UpdateColor()
    {
        Color c = Color.magenta;

        switch (state)
        {
            case CellState.Empty:
                c = emptyColor;
                break;
            case CellState.SnakeBody:
                c = snakeBodyColor;
                break;
            case CellState.SnakeHead:
                c = snakeHeadColor;
                break;
            case CellState.Apple:
                c = appleColor;
                break;
        }

        image.color = c;
    }
}
