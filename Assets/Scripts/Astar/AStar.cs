using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://itmining.tistory.com/66

[System.Serializable]
public class Astar_Node
{
    public bool Isobstacle;
    public int X;
    public int Y;

    public int G, H, F; // G 시작점 부터 현재 좌표까지 비용   
                 //H 현재 좌표에서 목적지 까지의 예상 이동 비용   
                 // F 현재까지 이동하는데 걸린 비용과 예상 비용을 합친 총 비용

    public Astar_Node ParentNode; //부모 노드

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
    public bool allowDiagonal = true, dontCrossCorner = true;


    private void Start()
    {
        //Size = new Vector2Int(25, 25);
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




    public List<Astar_Node> FindPath(Transform _Start, Transform _End)
    {
        Vector2Int Start = ChangePostoGrid(_Start);
        Vector2Int End = ChangePostoGrid(_End);

        Astar_Node StartNode, EndNode, CurNode;

        List<Astar_Node> OpenList;
        List<Astar_Node> ClosedList;
        List<Astar_Node> FinalNodeList;

        StartNode = grid[Start.x, Start.y];
        EndNode = grid[End.x, End.y];

        OpenList = new List<Astar_Node>(); //탐색한 노드
        ClosedList = new List<Astar_Node>();
        FinalNodeList = new List<Astar_Node>();
        OpenList.Add(grid[Start.x, Start.y]);

        while(OpenList.Count>0)
        {
            // OpenList중 가장 F가 작고 F가 같다면 H가 작은 것을 현재 노드로 하고 열린 리스트 -> 닫힌 리스트로 이동
            CurNode = OpenList[0];
            for(int i = 1; i< OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // 마지막
            if (CurNode == EndNode)
            {
                Astar_Node TargetCurNode = EndNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                //for (int i = 0; i < FinalNodeList.Count; i++)
                //    Debug.Log(i + "번째는 " + FinalNodeList[i].X + ", " + FinalNodeList[i].Y);
                return FinalNodeList;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.X + 1, CurNode.Y + 1, CurNode, EndNode, OpenList, ClosedList);
                OpenListAdd(CurNode.X - 1, CurNode.Y + 1, CurNode, EndNode, OpenList, ClosedList);
                OpenListAdd(CurNode.X - 1, CurNode.Y - 1, CurNode, EndNode, OpenList, ClosedList);
                OpenListAdd(CurNode.X + 1, CurNode.Y - 1, CurNode, EndNode, OpenList, ClosedList);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.X, CurNode.Y + 1, CurNode, EndNode, OpenList, ClosedList);
            OpenListAdd(CurNode.X + 1, CurNode.Y, CurNode, EndNode, OpenList, ClosedList);
            OpenListAdd(CurNode.X, CurNode.Y - 1, CurNode, EndNode, OpenList, ClosedList);
            OpenListAdd(CurNode.X - 1, CurNode.Y, CurNode, EndNode, OpenList, ClosedList);
        }

        return FinalNodeList;
    }

    void OpenListAdd(int checkX, int checkY, Astar_Node CurNode, Astar_Node EndNode, List<Astar_Node> OpenList, List<Astar_Node> ClosedList)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= 0 && checkX < gridSize.x  && checkY >= 0 && checkY < gridSize.y  && !grid[checkX , checkY ].Isobstacle 
            && !ClosedList.Contains(grid[checkX , checkY]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) 
                if (grid[CurNode.X , checkY ].Isobstacle && grid[checkX, CurNode.Y ].Isobstacle) 
                    return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) 
                if (grid[CurNode.X , checkY ].Isobstacle || grid[checkX , CurNode.Y ].Isobstacle) 
                    return;


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Astar_Node NeighborNode = grid[checkX , checkY ];
            int MoveCost = CurNode.G + (CurNode.X - checkX == 0 || CurNode.Y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.X - EndNode.X) + Mathf.Abs(NeighborNode.Y - EndNode.Y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }


    Vector2Int ChangePostoGrid(Transform Pos)
    {


        return new Vector2Int((int)(Pos.position.x / 10),(int)(Pos.position.z/10));
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
