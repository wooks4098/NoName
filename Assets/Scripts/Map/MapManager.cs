using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    [SerializeField] GameObject Planes;
    [SerializeField] GameObject Road;
    [SerializeField] GameObject Walls;
    [SerializeField] GameObject Doors;

    [SerializeField] GameObject PlanePrefab;
    Vector3Int PlaneSize;
    Vector3Int PlaneHalfSize;
    public Vector2Int PlaneCount;

    [SerializeField] GameObject WallPrefab;
    Vector3Int WallSize;
    BSPNode root;

     AStar astar;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        SetSize();
        SetAstar();
        GetComponent<BSP>().DivideNodde();
    }

    void SetAstar()
    {
        astar = new AStar();
        astar.SetGridSize(PlaneCount);
        astar.CreateGrid();
    }

    public void GetRoot(BSPNode _root)
    {
        root = _root;
    }
    void SetSize()
    {//에셋에 수정시 확인
        var boxCollider = PlanePrefab.GetComponent<BoxCollider>();
        var size = boxCollider.size;
        PlaneSize.x = (int)size.x;
        PlaneSize.y =0;
        PlaneSize.z = (int)size.z;

        PlaneHalfSize.x = (int)(PlaneSize.x * 0.5f);
        PlaneHalfSize.y = 0;
        PlaneHalfSize.z = (int)(PlaneSize.z * 0.5f);

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
    public void CreatPlane(BSPNode Node)
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
                    GameObject plane = Instantiate(PlanePrefab, Position, Quaternion.identity, Planes.transform);
                    Position.z += PlaneSize.z;

                }
                Position.x += PlaneSize.x;
                Position.z = bottomLeft.y;
            }

        }
      
        CreatPlane(Node.rightNode);
    }


    //깊이4 자식끼리 연결
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
                        Position.x = Node.parentNode.topRight.x - PlaneHalfSize.x;
                        Position.z = Node.parentNode.topRight.y - middle + PlaneHalfSize.z;


                    }
                    else
                    {
                        Position.x = Node.topRight.x - PlaneHalfSize.x;
                        Position.z = Node.topRight.y - middle + PlaneHalfSize.z;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity, Road.transform);
                        DeleteWall(Position);
                        astar.ChangeNode(false, (int)(Position.x - PlaneHalfSize.z) / PlaneSize.x, (int)(Position.z - PlaneHalfSize.z) / PlaneSize.z);
                        Position.x += PlaneSize.x;
                    }
                    break;
                case Direction.HORIZONTAL:
                    middle = Node.parentNode.rightNode.GetWidth() / 2;
                    middle -= middle % PlaneSize.z;
                    if (Node.parentNode.rightNode.topRight.y > Node.topRight.y)
                    {
                        Position.x = Node.parentNode.rightNode.bottomLeft.x+ middle + PlaneHalfSize.x;
                        Position.z = Node.parentNode.rightNode.bottomLeft.y - PlaneHalfSize.z;


                    }
                    else
                    {
                        Position.x = Node.bottomLeft.x + middle+ PlaneHalfSize.x;
                        Position.z = Node.bottomLeft.y- PlaneHalfSize.z;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity, Planes.transform);
                        DeleteWall(Position);
                        astar.ChangeNode(false, (int)(Position.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position.z - PlaneHalfSize.z) / PlaneSize.z);
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


    //깊이 2,3 자식끼리 연결
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
                        Position.x = Node.parentNode.leftNode.topRight.x - PlaneSize.x + PlaneHalfSize.x;
                        Position.z = Node.parentNode.leftNode.bottomLeft.y + PlaneSize.z + PlaneHalfSize.z;

                    }
                    else
                    {
                        Position.x = Node.parentNode.rightNode.topRight.x - PlaneSize.x + PlaneHalfSize.x;
                        Position.z = Node.parentNode.rightNode.bottomLeft.y + PlaneSize.z + PlaneHalfSize.z;


                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity, Road.transform);
                        DeleteWall(Position);
                        astar.ChangeNode(false, (int)(Position.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position.z - PlaneHalfSize.z) / PlaneSize.z);
                        Position.x += PlaneSize.x;

                    }
                    Node.isRoad = true;
                    Node.parentNode.rightNode.isRoad = true;
                    break;
                case Direction.HORIZONTAL:
                    if (Node.parentNode.leftNode.bottomLeft.y > Node.parentNode.rightNode.bottomLeft.y)
                    {

                        Position.x = Node.parentNode.rightNode.bottomLeft.x + PlaneSize.x + PlaneHalfSize.x;
                        Position.z = Node.parentNode.rightNode.topRight.y - PlaneSize.z + PlaneHalfSize.z;
                    }
                    else
                    {

                        Position.x = Node.parentNode.leftNode.bottomLeft.x + PlaneSize.x + PlaneHalfSize.x;
                        Position.z = Node.parentNode.leftNode.topRight.y - PlaneSize.z + PlaneHalfSize.z;

                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Instantiate(PlanePrefab, Position, Quaternion.identity, Road.transform);
                        DeleteWall(Position);
                        astar.ChangeNode(false, (int)(Position.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position.z - PlaneHalfSize.z) / PlaneSize.z);
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
        ConnetRoom2(Node.rightNode);
    }

    //Wall 생성
    public void CreateWall(BSPNode Node)
    {
        if (Node == null)
            return;

        CreateWall(Node.leftNode);
        if(Node.rightNode == null)
        {
            Vector3Int Position1 = Vector3Int.zero;
            Vector3Int Position2 = Vector3Int.zero;

            //Horizental 생성
            Position1.x = Node.bottomLeft.x + PlaneHalfSize.x;
            Position1.y = (int)(WallSize.y * 0.5f);
            Position1.z = Node.topRight.y + PlaneHalfSize.z - WallSize.z;

            Position2.x = Node.bottomLeft.x + PlaneHalfSize.x;
            Position2.y = (int)(WallSize.y * 0.5f);
            Position2.z = Node.bottomLeft.y + PlaneHalfSize.z;// - WallSize.z;

            for(int i = 0; i<Node.GetWidth() / WallSize.x; i++)
            {
                Instantiate(WallPrefab, Position1, Quaternion.identity, Walls.transform);
                Instantiate(WallPrefab, Position2, Quaternion.identity, Walls.transform);

                astar.ChangeNode(true, (int)(Position1.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position1.z - PlaneHalfSize.z) / PlaneSize.z);
                astar.ChangeNode(true, (int)(Position2.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position2.z - PlaneHalfSize.z) / PlaneSize.z);

                Position1.x += WallSize.x;
                Position2.x += WallSize.x;

            }

            //Vertical 생성
            Position1.x = Node.bottomLeft.x + PlaneHalfSize.x;
            Position1.y = (int)(WallSize.y * 0.5f);
            Position1.z = Node.bottomLeft.y + PlaneHalfSize.z + WallSize.z;

            Position2.x = Node.topRight.x + PlaneHalfSize.x - WallSize.x;
            Position2.y = (int)(WallSize.y * 0.5f);
            Position2.z = Node.bottomLeft.y + PlaneHalfSize.z + WallSize.z;

            for(int i = 0; i<Node.GetHeight() / WallSize.z -1; i++)
            {
                Instantiate(WallPrefab, Position1, Quaternion.identity, Walls.transform);
                Instantiate(WallPrefab, Position2, Quaternion.identity, Walls.transform);

                astar.ChangeNode(true, (int)(Position1.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position1.z - PlaneHalfSize.z) / PlaneSize.z);
                astar.ChangeNode(true, (int)(Position2.x - PlaneHalfSize.x) / PlaneSize.x, (int)(Position2.z - PlaneHalfSize.z) / PlaneSize.z);

                Position1.z += WallSize.z;
                Position2.z += WallSize.z;
            }
        }

        CreateWall(Node.rightNode);
    }

    //길과 겹쳐진 벽 제거
    void DeleteWall(Vector3Int Position)
    {
        Position.y += (int)(WallSize.y * 0.5f);

        //Position 주변에 있는 콜라이더 검색
        Collider[] colls = Physics.OverlapSphere(Position, 1f);

        for(int i = 0; i<colls.Length; i++)
        {
            colls[i].transform.parent = Doors.transform;
            colls[i].isTrigger = true;
            colls[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        DrawGizmos_Astar();
    }

    void DrawGizmos_Astar()
    {
        if (astar == null)
            return;
        for (int x = 0; x < PlaneCount.x; x++)
        {
            for (int y = 0; y < PlaneCount.y; y++)
            {
                if (astar.Isobstacle(x, y))
                    Gizmos.color = Color.black;
                else
                    Gizmos.color = Color.blue;

                Gizmos.DrawWireCube(new Vector3(x * 10 + PlaneHalfSize.x, -1, y * 10 + PlaneHalfSize.z), new Vector3(10, 0, 10));
            }
        }
    }
}
