using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MarchingCube : MC_Algorithm
{
    public bool Only_Section = false;

    [HideInInspector] public Mesh MC_Mesh;
    [HideInInspector] public GameObject[] Points_GO;
    [HideInInspector] public Vector3[] AllPoints;
    [HideInInspector] public List<int> MC_Triangles_Result;

    private Material pointMat;

    private void Awake()
    {
        pointMat = new(Shader.Find("Standard"));
        pointMat.color = MC_Color.Yellow_normal;

        if(this.transform.childCount < 8)
        {
            // Instantiate 8 Points of the MarchingCube
            for (int i = 0; i < 8; i++)
            {
                GameObject P = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DestroyImmediate(P.GetComponent<SphereCollider>());
                P.name = "Point " + i;
                P.transform.parent = this.transform;

                if (i == 0) P.transform.localPosition = new Vector3(-0.5f, 0.5f, 0.5f);
                else if (i == 1) P.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
                else if (i == 2) P.transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f);
                else if (i == 3) P.transform.localPosition = new Vector3(-0.5f, 0.5f, -0.5f);
                else if (i == 4) P.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f);
                else if (i == 5) P.transform.localPosition = new Vector3(0.5f, -0.5f, 0.5f);
                else if (i == 6) P.transform.localPosition = new Vector3(0.5f, -0.5f, -0.5f);
                else if (i == 7) P.transform.localPosition = new Vector3(-0.5f, -0.5f, -0.5f);

                P.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                P.GetComponent<MeshRenderer>().material = pointMat;
            }
        }

        // Get the MarchingCube's 8 Points' GameObjects
        Points_GO = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            Points_GO[i] = transform.GetChild(i).gameObject;
        }
    }

    public void Build_MarchingCube()
    {
        #region Get Marching Cube's Points & MidPoints' localPosition
        AllPoints = new Vector3[20];

        // Points
        for (int i = 0; i < 8; i++)
        {
            AllPoints[i] = Points_GO[i].transform.localPosition;
        }

        // MidPoints
        for (int i = 8; i < 20; i++)
        {
            if(i < 11) AllPoints[i] = (AllPoints[i - 8] + AllPoints[i - 7]) / 2;
            if(i == 11) AllPoints[i] = (AllPoints[3] + AllPoints[0]) / 2;
            if(i > 11 && i < 16) AllPoints[i] = (AllPoints[i - 12] + AllPoints[i - 8]) / 2;
            if(i >= 16 && i < 19) AllPoints[i] = (AllPoints[i - 12] + AllPoints[i - 11]) / 2;
            if(i == 19) AllPoints[i] = (AllPoints[7] + AllPoints[4]) / 2;
        }
        #endregion

        #region Rebuild Mesh, make MC_Mesh Angled
        MC_Mesh = new();
        MC_Mesh.name = "Basic Marching Cube";

        List<Vector3> Angled_MC_Vertices = new();
        List<int> Angled_MC_Triangles = new();
        for (int i = 0; i < MC_Data.BasicTriangles.Length; i++)
        {
            Angled_MC_Vertices.Add(AllPoints[MC_Data.BasicTriangles[i]]);
            Angled_MC_Triangles.Add(Angled_MC_Vertices.Count - 1);
        }

        MC_Mesh.vertices = Angled_MC_Vertices.ToArray();
        MC_Mesh.triangles = Angled_MC_Triangles.ToArray();
        MC_Mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = MC_Mesh;
        GetComponent<MeshRenderer>().sharedMaterial.color = MC_Color.Grey_normal;
        #endregion
    }

    public void Delete_Selected_Point()
    {
        #region Get Marching Cube's Points & MidPoints' localPosition
        AllPoints = new Vector3[20];

        // Points
        for (int i = 0; i < 8; i++)
        {
            AllPoints[i] = Points_GO[i].transform.localPosition;
        }

        // MidPoints
        for (int i = 8; i < 20; i++)
        {
            if (i < 11) AllPoints[i] = (AllPoints[i - 8] + AllPoints[i - 7]) / 2;
            if (i == 11) AllPoints[i] = (AllPoints[3] + AllPoints[0]) / 2;
            if (i > 11 && i < 16) AllPoints[i] = (AllPoints[i - 12] + AllPoints[i - 8]) / 2;
            if (i >= 16 && i < 19) AllPoints[i] = (AllPoints[i - 12] + AllPoints[i - 11]) / 2;
            if (i == 19) AllPoints[i] = (AllPoints[7] + AllPoints[4]) / 2;
        }
        #endregion

        #region Find DeletePoints & KeepPoints
        List<int> DeletePointsID = new();
        List<int> KeepPointsID = new();

        if (Points_GO.Length == 0)
        {
            Debug.LogError("Please Build the MarchingCube First");
            return;
        }

        for (int i = 0; i < Points_GO.Length; i++)
        {
            if (Selection.Contains(Points_GO[i]))
            {
                DeletePointsID.Add(i);
            }
            else
            {
                KeepPointsID.Add(i);
            }
        }

        if (DeletePointsID.Count == 0)
        {
            Debug.LogError("Please Select a point.");
            return;
        }
        #endregion

        #region Reset MC_Mesh's Vertices and Triangles
        // Get the Max Value of MC_Data.BasicTriangles
        int BT_Max_Value = 0;
        for (int i = 0; i < MC_Data.BasicTriangles.Length; i++)
        {
            if (MC_Data.BasicTriangles[i] > BT_Max_Value) BT_Max_Value = MC_Data.BasicTriangles[i];
        }
        
        if (BT_Max_Value > MC_Mesh.vertexCount)
        {
            MC_Mesh.vertices = AllPoints;
            MC_Mesh.triangles = MC_Data.BasicTriangles;
        }
        else
        {
            MC_Mesh.triangles = MC_Data.BasicTriangles;
            MC_Mesh.vertices = AllPoints;
        }
        #endregion

        // Build Triangles with Face && FillFace Algorithm
        MC_Triangles_Result = new();

        if (!Only_Section) MC_Triangles_Result.AddRange(Rebuild_Faces(DeletePointsID));

        if (DeletePointsID.Count <= 4)
        {
            MC_Triangles_Result.AddRange(Fill_Faces(DeletePointsID, false));
        }
        else
        {
            MC_Triangles_Result.AddRange(Fill_Faces(KeepPointsID, true));
        }

        #region Rebuild MC_Mesh, make MC_Mesh Angled
        List<Vector3> Angled_MC_Vertices = new();
        List<int> Angled_MC_Triangles = new();

        for (int i = 0; i < MC_Triangles_Result.Count; i++)
        {
            Angled_MC_Vertices.Add(AllPoints[MC_Triangles_Result[i]]);
            Angled_MC_Triangles.Add(Angled_MC_Vertices.Count - 1);
        }

        // Get the Max Value of Angled_MC_Triangles
        int T_Max_Value = 0;
        for (int i = 0; i < Angled_MC_Triangles.Count; i++)
        {
            if (Angled_MC_Triangles[i] > T_Max_Value) T_Max_Value = Angled_MC_Triangles[i];
        }

        if (T_Max_Value > MC_Mesh.vertexCount)
        {
            MC_Mesh.vertices = Angled_MC_Vertices.ToArray();
            MC_Mesh.triangles = Angled_MC_Triangles.ToArray();
        }
        else
        {
            MC_Mesh.triangles = Angled_MC_Triangles.ToArray();
            MC_Mesh.vertices = Angled_MC_Vertices.ToArray();
        }

        MC_Mesh.RecalculateNormals();
        #endregion
    }

    public void ResetMarchingCube()
    {
        GetComponent<MeshFilter>().mesh = null;

        AllPoints = null;
        MC_Triangles_Result = null;
    }

    public void Export_Algorithm_Result()
    {
        string FilePath = Application.dataPath + "/Marching_Cube_Algorithm.txt";
        List<string> Result = new();
        List<string> Triangles_data = new();
        List<string> useTriangle_data = new();

        int preTriangleCount = 0;

        for(int i0 = 0; i0 < 2; i0++)
        {
            for (int i1 = 0; i1 < 2; i1++)
            {
                for (int i2 = 0; i2 < 2; i2++)
                {
                    for (int i3 = 0; i3 < 2; i3++)
                    {
                        for (int i4 = 0; i4 < 2; i4++)
                        {
                            for (int i5 = 0; i5 < 2; i5++)
                            {
                                for (int i6 = 0; i6 < 2; i6++)
                                {
                                    for (int i7 = 0; i7 < 2; i7++)
                                    {
                                        int[] PointsSituation = { i0, i1, i2, i3, i4, i5, i6, i7 };
                                        string PointsSituation_TS = "        /* " + "["
                                            + PointsSituation[0] + PointsSituation[1] + PointsSituation[2] + PointsSituation[3]
                                            + PointsSituation[4] + PointsSituation[5] + PointsSituation[6] + PointsSituation[7]
                                            + "] = ";

                                        if (Triangles_data.Count < 10) PointsSituation_TS += "00";
                                        else if (Triangles_data.Count < 100) PointsSituation_TS += "0";
                                        PointsSituation_TS += Triangles_data.Count + " */     ";

                                        #region Find DeletePoints & KeepPoints
                                        List<int> DeletePointsID = new();
                                        List<int> KeepPointsID = new();
                                        for (int i = 0; i < PointsSituation.Length; i++)
                                        {
                                            if (PointsSituation[i] == 0)
                                            {
                                                DeletePointsID.Add(i);
                                            }
                                            else
                                            {
                                                KeepPointsID.Add(i);
                                            }
                                        }
                                        #endregion

                                        #region Build Triangles with Face && FillFace Algorithm
                                        MC_Triangles_Result = new();

                                        if (DeletePointsID.Count <= 4)
                                        {
                                            MC_Triangles_Result.AddRange(Fill_Faces(DeletePointsID, false));
                                        }
                                        else
                                        {
                                            MC_Triangles_Result.AddRange(Fill_Faces(KeepPointsID, true));
                                        }
                                        #endregion

                                        int[] Triangles = MC_Triangles_Result.ToArray();
                                        string Triangles_TS, useTriangle_TS;

                                        useTriangle_TS = preTriangleCount + ", " + Triangles.Length;

                                        if (Triangles.Length == 0)
                                        {
                                            Triangles_TS = "/* Triangle = null */";
                                        }
                                        else
                                        {
                                            Triangles_TS = Triangles[0] + ", ";
                                            for (int i = 1; i < Triangles.Length; i++)
                                            {
                                                Triangles_TS += Triangles[i];
                                                if (i != Triangles.Length - 1) Triangles_TS += ", ";
                                            }
                                        }

                                        if(Triangles_data.Count == 0)
                                        {
                                            Triangles_data.Add(PointsSituation_TS + Triangles_TS);
                                            useTriangle_data.Add(PointsSituation_TS + useTriangle_TS + ",");
                                        }
                                        else if (Triangles_data.Count > 0 && Triangles_data.Count < Mathf.Pow(2, 8) - 1)
                                        {
                                            Triangles_data.Add(PointsSituation_TS + Triangles_TS + ",");
                                            useTriangle_data.Add(PointsSituation_TS + useTriangle_TS + ",");
                                        }
                                        else
                                        {
                                            Triangles_data.Add(PointsSituation_TS + Triangles_TS);
                                            useTriangle_data.Add(PointsSituation_TS + useTriangle_TS);
                                        }

                                        preTriangleCount += Triangles.Length;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Result.Add("// TransfromTriangles data");
        Result.AddRange(Triangles_data.ToArray());        
        Result.Add("\n \n \n \n \n" + "// Informations to use TransfromTriangles data   [Start ID] , [Item Count]");
        Result.AddRange(useTriangle_data.ToArray());
        
        File.WriteAllLines(FilePath, Result.ToArray());
        Debug.Log("File Saved: " + FilePath);
    }
}
