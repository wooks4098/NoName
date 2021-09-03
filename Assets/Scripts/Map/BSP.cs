using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BSP : MonoBehaviour
{
    BSPNode root;
    [SerializeField] GameObject Plane;
    [SerializeField] Vector2Int PlaneCount;
    Vector2Int MapSize;
    private void Start()
    {
        SetMapSize();
        root = new BSPNode(Vector2Int.zero, MapSize);
    }

    void SetMapSize()
    {
        MapSize.x = (int)Plane.GetComponent<BoxCollider>().size.x * PlaneCount.x;
        MapSize.y = (int)Plane.GetComponent<BoxCollider>().size.z * PlaneCount.y;

    }

    private void OnDrawGizmos()
    {

        inorder(root);
    }

    void inorder(BSPNode ptr)
    {
        if (ptr!= null)
        {
            inorder(ptr.leftNode);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(ptr.bottomLeft.x + ptr.GetWidth() / 2, 0, ptr.bottomLeft.y + ptr.GetHeight() / 2), new Vector3(ptr.GetWidth(), 0, ptr.GetHeight()));
            inorder( ptr.rightNode);
        }
    }
}
