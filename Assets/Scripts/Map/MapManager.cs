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
                    Instantiate(PlanePrefab, Position, Quaternion.identity);
                    Position.z += 10;

                }
                Position.x += 10;
                Position.z = bottomLeft.y;
            }

        }
       
        CreatPlane(Node.rightNode);

    }
}
