using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    VERTICAL, HORIZONTAL
}

public class BSPNode  //상속체크
{
    public Vector2Int bottomLeft, topRight;
    public List<Vector2Int> roadbottonLeft;
    public List<Vector2Int> roadbottonRight;
    public bool isRoad = false;

    public BSPNode parentNode; 
    public BSPNode leftNode, rightNode;

    public bool isDivided = false; //방을 나누었는지 확인

    private Direction direction;



    public BSPNode(Vector2Int bottomLeft, Vector2Int topRight)
    {//생성자
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
    }

    

    public void SetDireciton()
    {

        if (GetWidth() > GetHeight())
            direction = Direction.VERTICAL;

        else if (GetWidth() < GetHeight())
            direction = Direction.HORIZONTAL;

        else
        {
            int temp = Random.Range(1, 2);
            if (temp == 1)
                direction = Direction.VERTICAL;
            else
                direction = Direction.HORIZONTAL;
        }
    }



    public bool GetIsDirection()
    {
        return (direction.Equals(Direction.VERTICAL));
        
    }

    public Direction GetDirection()
    {
        return direction;
    }


    public void DivideNode(int _depth = 0)//int ParentminSize
    {
        if (_depth >= 3)
            return;

        float temp;
        int dividedRatio = Random.Range(40, 55);
        Vector2Int divideLine1, divideLine2;

        SetDireciton();
        if (direction == Direction.VERTICAL)//switch
        {
            temp = (GetWidth()); //topRight.x - bottomLeft.x 
            temp = temp * dividedRatio * 0.01f;

            int width = Mathf.RoundToInt(temp);
            width -= width % 10;
            /*
                        if (width < ParentminSize || GetWidth() - width < ParentminSize)
                            return false;*/
            divideLine1 = new Vector2Int(bottomLeft.x + width, topRight.y);
            divideLine2 = new Vector2Int(bottomLeft.x + width, bottomLeft.y);

        }
        else
        {
            temp = (GetHeight()); //topRight.y - bottomLeft.y
            temp = temp * dividedRatio * 0.01f;

            int height = Mathf.RoundToInt(temp);
            height -= height % 10;
            /*if (height < ParentminSize || GetHeight() - height < ParentminSize)
                return false;*/
            divideLine1 = new Vector2Int(topRight.x, bottomLeft.y + height);
            divideLine2 = new Vector2Int(bottomLeft.x, bottomLeft.y + height);
        }

        leftNode = new BSPNode(bottomLeft, divideLine1);
        rightNode = new BSPNode(divideLine2, topRight);
        leftNode.parentNode = rightNode.parentNode = this;
        isDivided = true;

        leftNode.DivideNode(_depth+1);
        rightNode.DivideNode( _depth + 1);

    }

    public int GetHeight()
    {
        return topRight.y - bottomLeft.y;
    }

    public int GetWidth()
    {
        return topRight.x - bottomLeft.x;
    }

    /*    public void CreateRoom()
        {
            int distanceFrom = 2;
            if (!isDivided)
            {
                roomBL = new Vector3Int(bottomLeft.x + distanceFrom, bottomLeft.y + distanceFrom, 0);
                roomTR = new Vector3Int(topRight.x - distanceFrom, topRight.y - distanceFrom, 0);
            }
        }*/
}
/*    public bool DivideNode(int ParentminSize)
    {
        float temp;
        int dividedRatio = Random.Range(40, 70);
        Vector2Int divideLine1, divideLine2;
        if (depth >= 3)
            return false;
        SetDireciton();
        if (direction == Direction.VERTICAL)//switch
        {
            temp = (GetWidth()); //topRight.x - bottomLeft.x 
            temp = temp * dividedRatio  * 0.01f;

            int width = Mathf.RoundToInt(temp);
*//*
            if (width < ParentminSize || GetWidth() - width < ParentminSize)
                return false;*//*
            divideLine1 = new Vector2Int(bottomLeft.x + width, topRight.y);
            divideLine2 = new Vector2Int(bottomLeft.x + width, bottomLeft.y);

        }
        else
        {
            temp = (GetHeight()); //topRight.y - bottomLeft.y
            temp = temp * dividedRatio * 0.01f;

            int height = Mathf.RoundToInt(temp);

            *//*if (height < ParentminSize || GetHeight() - height < ParentminSize)
                return false;*//*
            divideLine1 = new Vector2Int(topRight.x, bottomLeft.y + height);
            divideLine2 = new Vector2Int(bottomLeft.x, bottomLeft.y + height);
        }

        leftNode = new BSPNode(bottomLeft, divideLine1,depth +1);
        rightNode = new BSPNode(divideLine2, topRight, depth + 1);
        leftNode.parentNode = rightNode.parentNode = this;
        isDivided = true;

        return true;
    }*/