using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake
{
    public Vector2 head;
    public List<Vector2> body = new List<Vector2>();
    private GridHandler handler;
    private Vector2 moveMax;
    private Vector2 moveDir;

    private Queue<Vector2> moveDirs = new Queue<Vector2>();

    public GridHandler Handler => handler;

    public Vector2 MoveDir
    { 
        get => moveDir;
        set
        {
            if (value.magnitude <= 1f)
            {
                if (value != MoveDir && value != MoveDir * -1f)
                    moveDir = value;
            }
        }
    }

    public Snake(Vector2 startPos, GridHandler handler) => (head, this.handler, moveMax) = (startPos, handler, handler.GridSize);

    public void EnqueueMove(Vector2 moveDir) => moveDirs.Enqueue(moveDir);
    
    public bool Move()
    {
        if (moveDirs.Count > 0)
            MoveDir = moveDirs.Dequeue();
        
        if (moveDir.magnitude == 0f) //this only passes at the start
            return true;
        
        Vector2 prevPos = head;
        
        head += MoveDir;
        if (head.x >= moveMax.x) 
            head.x = 0;
        else if (head.y >= moveMax.y) 
            head.y = 0;
        else if (head.x < 0) 
            head.x = moveMax.x - 1;
        else if (head.y < 0) 
            head.y = moveMax.y - 1;

        Cell newCell = handler.GetCell(head);
        if (newCell.State == CellState.Apple)
        {
            body.Add(head);
            handler.EatApple();
        }
        
        //todo because we do the body after the head, the head can hit the tip of the tail when it shouldnt
        if (body.Count > 0)
        {
            handler.GetCell(body.Last()).State = CellState.Empty;
            body.RemoveAt(body.Count - 1);
            body.Insert(0, prevPos);
            handler.GetCell(prevPos).State = CellState.SnakeBody; // i probably dont need to do this    
        }
        else
            handler.GetCell(prevPos).State = CellState.Empty;
        
        if (newCell.State == CellState.SnakeBody)
            return false; //player hit body, death
        
        newCell.State = CellState.SnakeHead;
        return true;
    }
}
