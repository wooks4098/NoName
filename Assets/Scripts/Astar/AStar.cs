using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://itmining.tistory.com/66
public class Astar_Node
{
    public bool Isobstacle;
    public int X;
    public int Y;
    int g, h, f;//거리 코스트
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
    Vector2Int Size;

    private void Start()
    {
        Size = new Vector2Int(25, 25);
        SetGridSize(new Vector2Int(Size.x, Size.y));
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




    void FindPath(Vector2 _Start, Vector2 _End)
    {
        Vector2Int Start = ChangePostoGrid(_Start);
        Vector2Int End = ChangePostoGrid(_End);

        Astar_Node StartNode, EndNode, CurNode;
        StartNode = grid[Start.x, Start.y];
        EndNode = grid[End.x, End.y];

        List<Astar_Node> OpenList = new List<Astar_Node>(); //탐색한 노드
        HashSet<Astar_Node> ClosedList = new HashSet<Astar_Node>();
        //List<Astar_Node> FinalNodeList = new List<Astar_Node>();
        OpenList.Add(grid[Start.x, Start.y]);


    }




    Vector2Int ChangePostoGrid(Vector2 Pos)
    {
        return new Vector2Int((int)(Pos.x / 10),(int)(Pos.y/10));
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
