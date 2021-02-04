using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Color emptyColor;
    public Color snakeColor;
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
            case CellState.Snake:
                c = snakeColor;
                break;
            case CellState.Apple:
                c = appleColor;
                break;
        }

        image.color = c;
    }
}
