using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BSP))]
public class BSPGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BSP generator = (BSP)target;
        if(GUILayout.Button("Create Cube"))
        {
            generator.DivideNodde();
        }

    }
}
