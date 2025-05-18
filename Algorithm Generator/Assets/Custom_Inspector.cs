using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//[CustomEditor(typeof(MarchingCube))]
public class Custom_Inspector : Editor
{
    #region UI
    public struct MC_Color
    {
        public static Color Yellow_light = new(255 / 255f, 255 / 255f, 200 / 255f);
        public static Color Yellow_normal = new(255 / 255f, 255 / 255f, 150 / 255f);
        public static Color Yellow_medium = new(205 / 255f, 205 / 255f, 100 / 255f);
        public static Color Yellow_Dark = new(155 / 255f, 155 / 255f, 50 / 255f);

        public static Color Grey_light = new(200 / 255f, 200 / 255f, 200 / 255f);
        public static Color Grey_normal = new(150 / 255f, 150 / 255f, 150 / 255f);
        public static Color Grey_medium = new(100 / 255f, 100 / 255f, 100 / 255f);
        public static Color Grey_Dark = new(40 / 255f, 40 / 255f, 40 / 255f);
    }
    public struct MC_Font
    {
        public static GUIStyle Area_Head = Area_Head_Style();
        static GUIStyle Area_Head_Style()
        {
            GUIStyle Style = new();
            Style.fontSize = 15;
            Style.fontStyle = FontStyle.Bold;
            Style.normal.textColor = MC_Color.Grey_light;
            Style.alignment = TextAnchor.MiddleCenter;
            return Style;
        }

        public static GUIStyle SubArea_Head = SubArea_Head_Style();
        static GUIStyle SubArea_Head_Style()
        {
            GUIStyle Style = new();
            Style.fontSize = 12;
            Style.fontStyle = FontStyle.Bold;
            Style.normal.textColor = MC_Color.Grey_light;
            Style.alignment = TextAnchor.MiddleCenter;
            return Style;
        }

        // ReadOnly content
        public static GUIStyle ReadOnly_Head = ReadOnly_Head_Style();
        static GUIStyle ReadOnly_Head_Style()
        {
            GUIStyle Style = new();
            Style.fontSize = 12;
            Style.fontStyle = FontStyle.Bold;
            Style.normal.textColor = MC_Color.Grey_normal;
            Style.alignment = TextAnchor.MiddleCenter;
            return Style;
        }
        public static GUIStyle ReadOnly_Body = ReadOnly_Body_Style();
        static GUIStyle ReadOnly_Body_Style()
        {
            GUIStyle Style = new();
            Style.fontSize = 12;
            Style.fontStyle = FontStyle.Normal;
            Style.normal.textColor = MC_Color.Grey_normal;
            Style.alignment = TextAnchor.MiddleCenter;
            return Style;
        }

        // Editable content
        public static GUIStyle Editable_Head = Editable_Head_Style();
        static GUIStyle Editable_Head_Style()
        {
            GUIStyle Style = new();
            Style.fontSize = 12;
            Style.fontStyle = FontStyle.Normal;
            Style.normal.textColor = MC_Color.Grey_light;
            Style.alignment = TextAnchor.MiddleCenter;
            return Style;
        }

    }
    #endregion

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MarchingCube MC = (MarchingCube)target;

        #region Set Area
        int StartMargin = 20;
        int EndMargin = 20;
        float PanelWidth = EditorGUIUtility.currentViewWidth - StartMargin - EndMargin;

        Rect Parameters_Area = new(StartMargin, 40, PanelWidth, 150);
        //Rect Instruction_Area = new(StartMargin, 40, PanelWidth, 150);

        GUILayout.Space(800);
        #endregion

        #region Parameters Area
        GUILayout.BeginArea(Parameters_Area);

        #region "Instruction" Lable
        GUILayout.BeginHorizontal();
        EditorGUI.DrawRect(new Rect(0, 10, PanelWidth / 2 - 50, 1), MC_Color.Grey_normal);
        GUILayout.Label("Parameters", MC_Font.Area_Head);
        EditorGUI.DrawRect(new Rect(PanelWidth / 2 + 50, 10, PanelWidth / 2 - 40, 1), MC_Color.Grey_normal);
        GUILayout.EndHorizontal();
        #endregion



        GUILayout.EndArea();
        #endregion

        //if (GUILayout.Button("Build a basic Marching Cube"))
        //{
        //    MCA.Build_MarchingCube();
        //}
        //
        //if (GUILayout.Button("Delete Selected Points"))
        //{
        //    MCA.DeletePoint();
        //}
        //
        //GUILayout.Space(10);
        //
        //if (GUILayout.Button("Reset Marching Cube"))
        //{
        //    MCA.ResetMarchingCube();
        //}
        //
        //GUILayout.Space(50);
        //
        //if (GUILayout.Button("Export Marching Cube Algorithm Result"))
        //{
        //    MCA.ExportAlgorithmResult();
        //}
        //if (GUILayout.Button("Export Marching Cube Algorithm Result For ComputeShader"))
        //{
        //    MCA.ExportAlgorithmResult_forComputeShader();
        //}
    }
}
