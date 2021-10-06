using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://itmining.tistory.com/66
public class Astar_Node
{
    public bool Isobstacle;
    public int X;
    public int Y;

    public int G, H, F; // G ������ ���� ���� ��ǥ���� ���   
                 //H ���� ��ǥ���� ������ ������ ���� �̵� ���   
                 // F ������� �̵��ϴµ� �ɸ� ���� ���� ����� ��ģ �� ���

    public Astar_Node ParentNode; //�θ� ���

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
    public bool allowDiagonal, dontCrossCorner;

    List<Astar_Node> OpenList;
    List<Astar_Node> ClosedList;
    List<Astar_Node> FinalNodeList;

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




    void FindPath(Vector2 _Start, Vector2 _End)
    {
        Vector2Int Start = ChangePostoGrid(_Start);
        Vector2Int End = ChangePostoGrid(_End);

        Astar_Node StartNode, EndNode, CurNode;
        StartNode = grid[Start.x, Start.y];
        EndNode = grid[End.x, End.y];

        OpenList = new List<Astar_Node>(); //Ž���� ���
        ClosedList = new List<Astar_Node>();
        FinalNodeList = new List<Astar_Node>();
        OpenList.Add(grid[Start.x, Start.y]);

        while(OpenList.Count>0)
        {
            // OpenList�� ���� F�� �۰� F�� ���ٸ� H�� ���� ���� ���� ���� �ϰ� ���� ����Ʈ -> ���� ����Ʈ�� �̵�
            CurNode = OpenList[0];
            for(int i = 1; i< OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // ������
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

                for (int i = 0; i < FinalNodeList.Count; i++)
                    Debug.Log(i + "��°�� " + FinalNodeList[i].X + ", " + FinalNodeList[i].Y);
                return;
            }


            // �֢آע�
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.X + 1, CurNode.Y + 1, CurNode);
                OpenListAdd(CurNode.X - 1, CurNode.Y + 1, CurNode);
                OpenListAdd(CurNode.X - 1, CurNode.Y - 1, CurNode);
                OpenListAdd(CurNode.X + 1, CurNode.Y - 1, CurNode);
            }

            // �� �� �� ��
            OpenListAdd(CurNode.X, CurNode.Y + 1, CurNode);
            OpenListAdd(CurNode.X + 1, CurNode.Y, CurNode);
            OpenListAdd(CurNode.X, CurNode.Y - 1, CurNode);
            OpenListAdd(CurNode.X - 1, CurNode.Y, CurNode);
        }
    }

    void OpenListAdd(int checkX, int checkY, Astar_Node CurNode)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= 0 && checkX < gridSize.x + 1 && checkY >= 0 && checkY < gridSize.y + 1 && !NodeArray[checkX - 0, checkY - 0].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal) if (NodeArray[CurNode.X - 0, checkY - 0].isWall && NodeArray[checkX - 0, CurNode.Y - 0].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - 0, CurNode.Y -0].isWall) return;


            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            Astar_Node NeighborNode = NodeArray[checkX - 0, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.X - checkX == 0 || CurNode.Y - checkY == 0 ? 10 : 14);


            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
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
