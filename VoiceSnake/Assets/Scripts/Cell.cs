using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public CellState state = CellState.Empty;
    public Vector2 gridPos;

    //replace with button?
    private Image img;
    private PlayerSide playerSide;
    private SelectType selectType;
    private RectTransform[] arrows;
    
    public void Initialize(Vector2 gridPos, PlayerSide playerSide)
    {
        this.gridPos = gridPos;
        this.playerSide = playerSide;
        img = GetComponent<Image>();
        img.color = playerSide.Color;
        arrows = new RectTransform[] 
        {
            transform.GetChild(0).GetComponent<RectTransform>(), transform.GetChild(1).GetComponent<RectTransform>(),
            transform.GetChild(2).GetComponent<RectTransform>(), transform.GetChild(3).GetComponent<RectTransform>()
        };
        
        ResetArrows();
    }

    private void ResetArrows()
    {
        RectTransform rect = img.rectTransform;
        
        arrows[0].sizeDelta = -rect.sizeDelta;
        arrows[1].sizeDelta = -rect.sizeDelta;
        arrows[2].sizeDelta = -rect.sizeDelta;
        arrows[3].sizeDelta = -rect.sizeDelta;
        
        arrows[0].anchoredPosition = Vector2.up * rect.sizeDelta.y;
        arrows[1].anchoredPosition = Vector2.right * rect.sizeDelta.x;
        arrows[2].anchoredPosition = Vector2.down * -rect.sizeDelta.y;
        arrows[3].anchoredPosition = Vector2.left * -rect.sizeDelta.x;
    }

    public void OnClick()
    {
        if (GameHandler.CurrentPlayer == playerSide.PlayerIndex)
        {
            GameHandler.SelectedCell = this;

            if (GameHandler.CurrentState == GameState.Setup)
            {
                SelectType type = selectType == SelectType.Place ? SelectType.Rotate : SelectType.Place;
                Select(type);
            }
        }
    }

    public void Select(SelectType selectType)
    {
        switch (selectType)
        {
            case SelectType.Place:
                DisplayRotation(true, playerSide.AllowedRotations(gridPos));
                break;
            case SelectType.Rotate:
                // something
                break;
            case SelectType.None:
                DisplayRotation(false);
                break;
        }
    }

    private void DisplayRotation(bool show, Rotation[] rotations = null)
    {
        rotations = rotations ?? new Rotation[] {};
        
        if (show)
        {
            foreach (Rotation r in rotations)
                transform.GetChild((int)r).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        
    }
}