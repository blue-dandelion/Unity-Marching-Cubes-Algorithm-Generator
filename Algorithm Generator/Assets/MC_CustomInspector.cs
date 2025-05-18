using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MarchingCube))]
public class MC_CustomInspector : Editor
{
    MarchingCube MC;

    private void OnEnable()
    {
        MC = (MarchingCube)target;
        //SceneView.duringSceneGui += CustomOnSceneGUI;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("Build a basic Marching Cube"))
        {
            MC.Build_MarchingCube();
        }
        if (GUILayout.Button("Delete selected Points"))
        {
            MC.Delete_Selected_Point();
        }
        if (GUILayout.Button("Reset Marching Cube"))
        {
            MC.ResetMarchingCube();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Export Algorithm Result"))
        {
            MC.Export_Algorithm_Result();
        }
    }

    private void OnSceneGUI()
    {
        GUIStyle LableStyle = new();
        LableStyle.alignment = TextAnchor.MiddleCenter;
        LableStyle.normal.textColor = MC_Color.Yellow_normal;
        LableStyle.fontSize = 20;

        float Offset = 0.1f;
        Vector3[] Lable_Offsets = new Vector3[8]
        {
            new Vector3(-Offset , Offset, Offset),
            new Vector3(Offset, Offset, Offset),
            new Vector3(Offset, Offset, -Offset),
            new Vector3(-Offset, Offset, -Offset),
            new Vector3(-Offset , -Offset, Offset),
            new Vector3(Offset, -Offset, Offset),
            new Vector3(Offset, -Offset, -Offset),
            new Vector3(-Offset, -Offset, -Offset)
        };

        Handles.Label(MC.Points_GO[0].transform.position + Lable_Offsets[0], "P0", LableStyle);
        Handles.Label(MC.Points_GO[1].transform.position + Lable_Offsets[1], "P1", LableStyle);
        Handles.Label(MC.Points_GO[2].transform.position + Lable_Offsets[2], "P2", LableStyle);
        Handles.Label(MC.Points_GO[3].transform.position + Lable_Offsets[3], "P3", LableStyle);
        Handles.Label(MC.Points_GO[4].transform.position + Lable_Offsets[4], "P4", LableStyle);
        Handles.Label(MC.Points_GO[5].transform.position + Lable_Offsets[5], "P5", LableStyle);
        Handles.Label(MC.Points_GO[6].transform.position + Lable_Offsets[6], "P6", LableStyle);
        Handles.Label(MC.Points_GO[7].transform.position + Lable_Offsets[7], "P7", LableStyle);
    }
}
