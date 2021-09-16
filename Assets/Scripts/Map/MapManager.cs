using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }


    [SerializeField] GameObject PlanePrefab;
    Vector3Int PlaneSize;
    public Vector2Int PlaneCount;

    [SerializeField] GameObject WallPrefab;
    Vector3Int WallSize;
    BSPNode root;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        SetSize();
        GetComponent<BSP>().DivideNodde();
    }

    public void GetRoot(BSPNode _root)
    {
        root = _root;
        CreatPlane(root);

    }
    void SetSize()
    {//���¿� ������ Ȯ��
        var boxCollider = PlanePrefab.GetComponent<BoxCollider>();
        var size = boxCollider.size;
        PlaneSize.x = (int)size.x;
        PlaneSize.y =0;
        PlaneSize.z = (int)size.z;

        //boxCollider = WallPrefab.GetComponent<BoxCollider>();
        //size = boxCollider.size;
        WallSize.x = 10;
        WallSize.y = 10;
        WallSize.z = 10;

    }

    public Vector2Int GetMapsize()
    {
        Vector2Int Mapsize = Vector2Int.zero;
        Mapsize.x = PlaneSize.x * PlaneCount.x;
        Mapsize.y = PlaneSize.z * PlaneCount.y;
        return Mapsize;
    }

    //�ٴڻ���
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
            for (int i = 0; i < (topRight.x - bottomLeft.x) / PlaneSize.x + 1; i++)
            {
                for (int j = 0; j < (topRight.y - bottomLeft.y) / PlaneSize.z + 1; j++)
                {
                    GameObject plane = Instantiate(PlanePrefab, Position, Quaternion.identity,this.gameObject.transform);
                    Position.z += PlaneSize.z;

                }
                Position.x += PlaneSize.x;
                Position.z = bottomLeft.y;
            }

        }
      
        CreatPlane(Node.rightNode);
    }

    //�� �ٴ� ����

    //���� 3 �ڽĳ��� ����
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
                    middle -= middle % PlaneSize.z;
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
                        Position.x += PlaneSize.x;
                    }
                    break;
                case Direction.HORIZONTAL:
                    middle = Node.parentNode.rightNode.GetWidth() / 2;
                    middle -= middle % PlaneSize.z;
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
                        Instantiate(PlanePrefab, Position, Quaternion.identity, this.gameObject.transform);
                        Position.z += PlaneSize.z;
                    }

                    
                    break;
                default:
                    break;
            }
            Vector2Int test = Vector2Int.zero;
            Node.isRoad = true;
            Node.parentNode.rightNode.isRoad = true;
            
        }
        ConnetRoom(Node.rightNode);
    }

    public void ConnetRoom2(BSPNode Node)
    {
        if (Node == null)
            return;
        ConnetRoom2(Node.leftNode);
        if(Node != root && Node.isRoad != false&& Node == Node.parentNode.leftNode&&Node.leftNode != null)
        //if (Node != root && Node.leftNode.isRoad == true && Node == Node.parentNode.leftNode)
        {
            Vector3Int Position = Vector3Int.zero;
            switch (Node.parentNode.GetDirection())
            {
                case Direction.VERTICAL:
                    if (Node.bottomLeft.x > Node.parentNode.rightNode.bottomLeft.x)
                    {
                        if (Node.parentNode.rightNode.rightNode.topRight.y > Node.parentNode.rightNode.leftNode.topRight.y)
                        {
                            Position.x = Node.parentNode.rightNode.rightNode.topRight.x - 5;
                            Position.z = Node.parentNode.rightNode.rightNode.topRight.y - 5;
                        }
                        else
                        {
                            Position.x = Node.parentNode.rightNode.leftNode.topRight.x - 5;
                            Position.z = Node.parentNode.rightNode.leftNode.topRight.y - 5;
                        }

                    }
                    else
                    {
                        if (Node.rightNode.topRight.y > Node.leftNode.topRight.y)
                        {
                            Position.x = Node.rightNode.topRight.x - 5;
                            Position.z = Node.rightNode.topRight.y - 5;
                        }
                        else
                        {
                            Position.x = Node.leftNode.topRight.x - 5;
                            Position.z = Node.leftNode.topRight.y - 5;
                        }

                        //Position.x = Node.topRight.x - 5;
                        //Position.z = Node.topRight.y - 5;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity);
                        Position.z += PlaneSize.z;
                    }
                    Node.isRoad = true;
                    Node.parentNode.rightNode.isRoad = true;
                    break;
                case Direction.HORIZONTAL:
                    //if (Node.bottomLeft.y > Node.parentNode.rightNode.bottomLeft.y)
                    //{
                    //    Position.x = Node.bottomLeft.x + 5;
                    //    Position.z = Node.bottomLeft.y + 5;

                    //}
                    //else
                    //{
                    //    Position.x = Node.parentNode.rightNode.bottomLeft.x + 5;
                    //    Position.z = Node.parentNode.rightNode.bottomLeft.y + 5;
                    //}
                    //for (int i = 0; i < 2; i++)
                    //{
                    //    Instantiate(PlanePrefab, Position, Quaternion.identity);
                    //    Position.z += PlaneSize.z;
                    //}



                    break;
                default:
                    break;
            }

            return;
        }
        ConnetRoom2(Node.rightNode);
    }

    //����2 �ڽĳ��� ����

}
