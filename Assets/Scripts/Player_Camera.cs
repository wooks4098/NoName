using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    private void Update()
    {
        
    }


    void LookAround()//카메라 무빙
    {

        // transform.RotateAround()

        //Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * 3f, Input.GetAxis("Mouse Y") * 3f);//마우스 좌표 받기
        //Vector3 camAngle = cameraArm.rotation.eulerAngles;

        //float x = camAngle.x - mouseDelta.y;
        //if (x < 180f)
        //{
        //    x = Mathf.Clamp(x, -1f, 45f);
        //}
        //else
        //{
        //    // x = Mathf.Clamp(x, 335f, 361f);
        //    x = Mathf.Clamp(x, 345f, 371f);
        //}

        //cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
        //// transform.RotateAround()
    }
}
