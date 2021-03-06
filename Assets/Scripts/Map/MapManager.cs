using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Room
{
    //Room info
    public Vector2Int bottomLeft;
    public Vector2Int topRight;
    public float size;
    public List<Door> Door = new List<Door>();
    public bool isClear = false; //방을 클리어했는지

    //Monster info
    public List<MonsterController> monsterList = new List<MonsterController>(); //방에 있을 몬스터
}
public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    [SerializeField] int playerRoom;

    //플레이어가 이동한 문
    [SerializeField] int firstOutDoor = -1;
    [SerializeField] int secondOutDoor = -1;

    [SerializeField] GameObject Planes;
    [SerializeField] GameObject Road;
    [SerializeField] GameObject Walls;
    [SerializeField] GameObject Doors;
    [SerializeField] GameObject DoorColliders;

    [SerializeField] GameObject PlanePrefab; //바닥 프리펨
    Vector3Int PlaneSize; 
    Vector3Int PlaneHalfSize; 
    public Vector2Int PlaneCount; 

    [SerializeField] GameObject WallPrefab;
    Vector3Int WallSize;
    BSPNode root;

    [SerializeField] GameObject DoorPrefab;

    [SerializeField] List<Room> rooms;

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
        PlaneSize.y = 0;
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

    #region 맵 생성

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
                        Position.x = Node.parentNode.rightNode.bottomLeft.x + middle + PlaneHalfSize.x;
                        Position.z = Node.parentNode.rightNode.bottomLeft.y - PlaneHalfSize.z;


                    }
                    else
                    {
                        Position.x = Node.bottomLeft.x + middle + PlaneHalfSize.x;
                        Position.z = Node.bottomLeft.y - PlaneHalfSize.z;
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
        if (Node.leftNode != null && Node != root && Node.isRoad == false)
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
        if (Node.rightNode == null)
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

            for (int i = 0; i < Node.GetWidth() / WallSize.x; i++)
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

            for (int i = 0; i < Node.GetHeight() / WallSize.z - 1; i++)
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

        for (int i = 0; i < colls.Length; i++)
        {
            
            colls[i].isTrigger = true;
            colls[i].GetComponent<MeshRenderer>().enabled = false;
            DoorCollider doorCollider = colls[i].gameObject.AddComponent<DoorCollider>();
            doorCollider.transform.parent = DoorColliders.transform;
            //생성되었던 벽 제거
            //colls[i].gameObject.SetActive(false);
            //문 생성
            GameObject door = Instantiate(DoorPrefab, Position, Quaternion.identity, Doors.transform);
            //문 정보 입력
            DoorInfoSet(Position, door.GetComponent<Door>(), doorCollider);
        }
    }

    //문 정보 입력
    void DoorInfoSet(Vector3 _doorPos, Door _door, DoorCollider _doorCollider)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].bottomLeft.x - PlaneSize.x < _doorPos.x && _doorPos.x < rooms[i].topRight.x + PlaneSize.x &&
                rooms[i].bottomLeft.y - PlaneSize.z < _doorPos.z && _doorPos.z < rooms[i].topRight.y + PlaneSize.z)
            {
                rooms[i].Door.Add(_door);
                _door.SetRoomNumber(i);
                _doorCollider.SetDoorNumber(i);
                if (rooms[i].bottomLeft.y < _doorPos.z && _doorPos.z < rooms[i].topRight.y)
                    _door.gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            }
        }
    }
    #endregion

    #region 방 정보 입력



    //방 정보 입력(크기 순서)
    public void RoominfoSet()
    {
        RoomSizeSet(root);
        rooms.Sort((a, b) => (a.size < b.size) ? -1 : 1);
    }

   void RoomSizeSet(BSPNode _node)
    {
        if (_node == null)
            return;

        if(_node.rightNode == null)
        {
            Room room = new Room();
            room.bottomLeft = new Vector2Int(_node.bottomLeft.x + PlaneSize.x, _node.bottomLeft.y + PlaneSize.z) ;
            room.topRight = new Vector2Int(_node.topRight.x - PlaneSize.x, _node.topRight.y - PlaneSize.z);
            room.size = _node.GetHeight() * _node.GetWidth();
            rooms.Add(room);
        }
        RoomSizeSet(_node.leftNode);
        RoomSizeSet(_node.rightNode);

    }

    public void AddRoomMonster(int _roomNumber, MonsterController _monster)
    {
        rooms[_roomNumber].monsterList.Add(_monster);
    }

    public void RemoveRoomMonster( MonsterController _monster)
    {
        rooms[playerRoom].monsterList.Remove(_monster);
        if(rooms[playerRoom].monsterList.Count <= 0)
        {
            rooms[playerRoom].isClear = true;
            AllDoorOpen();
        }
    }

    void RoomNumberSet()
    {    
        Room temp;
        for(int i = 0; i < rooms.Count; i++)
        {
            for(int j = 0; j<rooms.Count-1; j++)
            {
                if(rooms[i].size < rooms[j].size)
                {
                    temp = rooms[i];
                    rooms[i] = rooms[j];
                    rooms[j] = temp;
                }
            }
        }
    }

    #endregion


    public void SetPlayerRoomNumber(int _number)
    {
        playerRoom = _number;
    }


    //모든 방 정보 리턴
    public List<Room> GetRooms()
    {
        return rooms;
    }
    //특정 방 정보 리턴
    public Room GetRoom(int _roomNumber)
    {
        return rooms[_roomNumber];
    }


        #region A*
        public List<Astar_Node> GetAstarPath(Transform _Start, Transform _End)
    {
        return astar.FindPath(_Start, _End);
    }


    #endregion

    public void PlayerExitRoom(int _roomNumber)
    {
        //나간 문이 같은 방의 문인 경우 리턴
        if (firstOutDoor == _roomNumber)
            return;


        if (firstOutDoor == -1)
        {//현재 위치한 방문 나감
            firstOutDoor = _roomNumber;
        }
        else if (secondOutDoor == -1)
        {//다른 방문 나감
            secondOutDoor = _roomNumber;
        }

        //현재 있는 방에 있으므로 리턴
        if (secondOutDoor == -1)
            return;

        //다른방으로 이동
        if(firstOutDoor != secondOutDoor)
        {
            playerRoom = secondOutDoor;
        }

        firstOutDoor = -1;
        secondOutDoor = firstOutDoor;


        //이동한 방이 클리어한 체크
        if (rooms[playerRoom].isClear)
            return;

        //방문 열리고 다른 방은 닫기

            OpenDoor(playerRoom);
        CloseDoor(playerRoom);
        SetActiveMonster();

    }
    void SetActiveMonster()
    {
        for(int i = 0; i<rooms[playerRoom].monsterList.Count; i++)
        {
            rooms[playerRoom].monsterList[i].gameObject.SetActive(true);
        }
    }

    public void AllDoorOpen()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms[i].Door.Count; j++)
            {
                rooms[i].Door[j].OpenDoor();
            }
        }
    }

    public void AllDoorClose()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms[i].Door.Count; j++)
            {
                rooms[i].Door[j].CloseDoor();
            }
        }
    }
    public void OpenDoor(int _roomNumber)
    {
        for(int i = 0; i< rooms[_roomNumber].Door.Count;i++)
        {
            rooms[_roomNumber].Door[i].OpenDoor();
        }
    }

    public void CloseDoor(int _otherRoomNumber)
    {
        for(int i = 0; i<rooms.Count; i++)
        {
            if (i == _otherRoomNumber)
                continue;
            for(int j = 0; j< rooms[i].Door.Count; j++)
            {
                rooms[i].Door[j].CloseDoor();
            }
        }
    }

    public int GetPlayerRoomNumber()
    {
        return playerRoom;
    }
    #region Gizmos


    private void OnDrawGizmosSelected()
    {
        DrawGizmos_Astar();
        //DrawGizoms_AstarPaht();
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

    void DrawGizoms_AstarPaht()
    {
        //List<Astar_Node> FinalNodeList = astar.FindPath(new Vector2(10, 10), new Vector2(230, 230));
        Gizmos.color = Color.white;
        /*if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(FinalNodeList[i].X * 10 +5f , 0.5f, FinalNodeList[i].Y * 10 + 5f), new Vector3(FinalNodeList[i + 1].X * 10 +5f, 0.5f, FinalNodeList[i + 1].Y * 10 +5f));*/
    }
    #endregion
}
