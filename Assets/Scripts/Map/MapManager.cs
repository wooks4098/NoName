using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    [SerializeField] GameObject PlanePrefab;
    BSPNode root;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

    }

    public void GetRoot(BSPNode _root)
    {
        root = _root;
        CreatPlane(root);

    }


    //官蹿积己
    void CreatPlane(BSPNode Node)
    {
        if (Node == null)
            return;
        CreatPlane(Node.leftNode);
        if (Node.leftNode == null)
        {

            Vector2Int bottomLeft, topRight;
            Vector3Int Position = Vector3Int.zero;
            Position.x = Node.bottomLeft.x + 15;
            Position.z = Node.bottomLeft.y + 15;

            bottomLeft = Node.bottomLeft + Vector2Int.one * 15;
            topRight = Node.topRight - Vector2Int.one * 15;
            for (int i = 0; i < (topRight.x - bottomLeft.x) / 10 + 1; i++)
            {
                for (int j = 0; j < (topRight.y - bottomLeft.y) / 10 + 1; j++)
                {
                    GameObject plane = Instantiate(PlanePrefab, Position, Quaternion.identity,this.gameObject.transform);
                    Position.z += 10;

                }
                Position.x += 10;
                Position.z = bottomLeft.y;
            }

        }
      
        CreatPlane(Node.rightNode);
    }

    //辨 官蹿 积己
    public void ConnetRoom(BSPNode Node)
    {
        if (Node == null)
            return;
        ConnetRoom(Node.leftNode);
        if (Node.leftNode == null && Node == Node.parentNode.leftNode)
        {
            Vector3Int Position = Vector3Int.zero;
            int middle;
            switch (Node.parentNode.GetDirection())
            {
                case Direction.VERTICAL:
                    middle = Node.parentNode.rightNode.GetHeight() / 2;
                    middle -= middle % 10;
                    if (Node.parentNode.bottomLeft.x < Node.bottomLeft.x)
                    {
                        Position.x = Node.parentNode.topRight.x - 5;
                        Position.z = Node.parentNode.topRight.y - middle + 5;
                    }
                    else
                    {
                        Position.x = Node.topRight.x - 5;
                        Position.z = Node.topRight.y - middle + 5;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity, this.gameObject.transform);
                        Position.x += 10;
                    }
                    break;
                case Direction.HORIZONTAL:
                    middle = Node.parentNode.rightNode.GetWidth() / 2;
                    middle -= middle % 10;
                    if (Node.parentNode.rightNode.topRight.y > Node.topRight.y)
                    {
                        Position.x = Node.parentNode.rightNode.bottomLeft.x+ middle + 5;
                        Position.z = Node.parentNode.rightNode.bottomLeft.y - 5;
                    }
                    else
                    {
                        Position.x = Node.bottomLeft.x + middle+ 5;
                        Position.z = Node.bottomLeft.y-5;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity);
                        Position.z += 10;
                    }

                    
                    break;
                default:
                    break;
            }
            

        }
        ConnetRoom(Node.rightNode);
    }
}
