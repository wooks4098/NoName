using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BSP : MonoBehaviour
{
    public BSPNode root;
    //[SerializeField] GameObject Plane;
    //[SerializeField] Vector2Int PlaneCount;
    private void Start()
    {
        //DivideNodde();
        /*Vector2Int MapSize = SetMapSize();
        root = new BSPNode(Vector2Int.zero, MapSize);*/
        //DivideNodde(root);
    }
    public void DivideNodde()
    {

        Vector2Int MapSize = MapManager.Instance.GetMapsize();
        root = new BSPNode(Vector2Int.zero, MapSize);
        root.DivideNode();
        MapManager.Instance.GetRoot(root);
        MapManager.Instance.CreatPlane(root);
        MapManager.Instance.CreateWall(root);
        MapManager.Instance.ConnetRoom(root);
        MapManager.Instance.ConnetRoom2(root);

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
