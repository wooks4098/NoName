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
    {//에셋에 수정시 확인
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

    //바닥생성
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

    //길 바닥 생성

    //깊이 3 자식끼리 연결
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
        if(Node.leftNode != null && Node != root && Node.isRoad == false)
        {
            Vector3Int Position = Vector3Int.zero;

            switch (Node.parentNode.GetDirection())
            {
                case Direction.VERTICAL:

                    if (Node.parentNode.leftNode.topRight.x < Node.parentNode.rightNode.topRight.x)
                    {
                        Position.x = Node.parentNode.leftNode.topRight.x - PlaneSize.x +5;
                        Position.z = Node.parentNode.leftNode.bottomLeft.y + PlaneSize.z + 5;

                    }
                    else
                    {
                        Position.x = Node.parentNode.rightNode.topRight.x - PlaneSize.x + 5;
                        Position.z = Node.parentNode.rightNode.bottomLeft.y + PlaneSize.z + 5;


                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity);
                        Position.x += PlaneSize.x;
                    }
                    Node.isRoad = true;
                    Node.parentNode.rightNode.isRoad = true;
                    break;
                case Direction.HORIZONTAL:
                    if (Node.parentNode.leftNode.bottomLeft.y > Node.parentNode.rightNode.bottomLeft.y)
                    {

                        Position.x = Node.parentNode.rightNode.bottomLeft.x + PlaneSize.x + 5;
                        Position.z = Node.parentNode.rightNode.topRight.y - PlaneSize.z + 5;
                    }
                    else
                    {

                        Position.x = Node.parentNode.leftNode.bottomLeft.x + PlaneSize.x + 5;
                        Position.z = Node.parentNode.leftNode.topRight.y - PlaneSize.z + 5;

                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity);
                        Position.z += PlaneSize.z;
                    }
                    Node.isRoad = true;
                    Node.parentNode.rightNode.isRoad = true;



                    break;
                default:
                    break;
            }

            return;
        }


/*        if(Node != root && Node.isRoad != false&& Node == Node.parentNode.leftNode&&Node.leftNode != null)
        //if (Node != root && Node.leftNode.isRoad == true && Node == Node.parentNode.leftNode)
        {
           
        }*/
        ConnetRoom2(Node.rightNode);
    }

    //깊이2 자식끼리 연결

}
