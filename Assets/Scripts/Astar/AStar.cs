using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar_Node
{
    public bool Isobstacle;
    public int X;
    public int Y;

    public Astar_Node(bool _Isobstacle, int _X, int _Y)
    {
        Isobstacle = _Isobstacle;
        X = _X;
        Y = _Y;
    }

    public void ChangeNode(bool _Isobstacle)
    {
        Isobstacle = _Isobstacle;
    }
}


public class AStar 
{
    Astar_Node[,] grid;
    Vector2Int gridSize;


    private void Start()
    {
        SetGridSize(new Vector2Int(25, 25));
        CreateGrid();
    }

    public void SetGridSize(Vector2Int _gridSize)
    {
        gridSize = _gridSize;
    }

    public void CreateGrid()
    {
        grid = new Astar_Node[gridSize.x, gridSize.y];

        for(int x = 0; x< gridSize.x; x++)
        {
            for(int y = 0; y< gridSize.y; y++)
            {
                grid[x, y] = new Astar_Node(false, x, y);
            }
        }
    }




    public bool Isobstacle(int x, int y)
    {
        return grid[x, y].Isobstacle;
    }

    public void ChangeNode(bool _Isobstacle,int x, int y)
    {
        if(x >= 0 && x<= gridSize.x && y >= 0 && y <= gridSize.y)
            grid[x, y].ChangeNode(_Isobstacle);

    }
}
