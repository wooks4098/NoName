using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BSP : MonoBehaviour
{
    public BSPNode root;
    [SerializeField] GameObject Plane;
    [SerializeField] Vector2Int PlaneCount;
    private void Start()
    {
        DivideNodde();
        /*Vector2Int MapSize = SetMapSize();
        root = new BSPNode(Vector2Int.zero, MapSize);*/
        //DivideNodde(root);
    }
    public void DivideNodde()
    {
        Vector2Int MapSize = SetMapSize();
        root = new BSPNode(Vector2Int.zero, MapSize);
        root.DivideNode();
        MapManager.Instance.GetRoot(root);
    }
    Vector2Int SetMapSize()
    {
        var boxCollider = Plane.GetComponent<BoxCollider>();
        var size = boxCollider.size;
        Vector2Int mapSize = Vector2Int.zero;
        mapSize.x = (int)size.x * PlaneCount.x;
        mapSize.y = (int)size.z * PlaneCount.y;
        return mapSize;
    }

    private void OnDrawGizmos()
    {

        inorder(root);
    }

    void inorder(BSPNode ptr)
    {
        if (ptr != null)
        {
            inorder(ptr.leftNode);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(ptr.bottomLeft.x + ptr.GetWidth() *0.5f, 0, ptr.bottomLeft.y + ptr.GetHeight() * 0.5f), new Vector3(ptr.GetWidth(), 0, ptr.GetHeight()));
            inorder(ptr.rightNode);
        }
    }
}
