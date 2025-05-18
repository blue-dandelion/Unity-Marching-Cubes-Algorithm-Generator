using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Algorithm : MonoBehaviour
{
    /* ------------------------------ Face Algorithm ------------------------------ */
    int[] Find_prePoint_nexPoint_preMidPoint_nexMidPoint(int[] FacePoints, int DeletePointID)
    {
        int preP, nexP;
        int preMidP = -1, nexMidP = -1;

        if (DeletePointID > 0) preP = FacePoints[DeletePointID - 1];
        else preP = FacePoints[3];

        if (DeletePointID < 3) nexP = FacePoints[DeletePointID + 1];
        else nexP = FacePoints[0];

        for (int m = 0; m < 3; m++)
        {
            for (int n = 0; n < 3; n++)
            {
                if (MC_Data.PconnMidPs[preP * 3 + m] == MC_Data.PconnMidPs[FacePoints[DeletePointID] * 3 + n])
                {
                    preMidP = MC_Data.PconnMidPs[preP * 3 + m];
                    break;
                }
            }

            if (preMidP != -1) break;
        }

        for (int m = 0; m < 3; m++)
        {
            for (int n = 0; n < 3; n++)
            {
                if (MC_Data.PconnMidPs[nexP * 3 + m] == MC_Data.PconnMidPs[FacePoints[DeletePointID] * 3 + n])
                {
                    nexMidP = MC_Data.PconnMidPs[nexP * 3 + m];
                    break;
                }
            }

            if (nexMidP != -1) break;
        }

        int[] value = { preP, nexP, preMidP, nexMidP };
        return value;
    }

    public int[] Rebuild_Faces(List<int> DeletePointsID)
    {
        List<int> Triangles = new();

        // Detect if the Face contains the DeletePoints
        for (int i = 0; i < 6; i++)
        {
            #region Find out how many DeletePoint is on this Face
            int[] FacePs = { MC_Data.FacePs[i * 4], MC_Data.FacePs[i * 4 + 1], MC_Data.FacePs[i * 4 + 2], MC_Data.FacePs[i * 4 + 3] };
            int[] FaceP_is_DeleteP = { 0, 0, 0, 0 };
            int DeleteP_Count = 0;

            if (DeletePointsID.Contains(FacePs[0]))
            {
                FaceP_is_DeleteP[0] = 1;
                DeleteP_Count++;
            }
            if (DeletePointsID.Contains(FacePs[1]))
            {
                FaceP_is_DeleteP[1] = 1;
                DeleteP_Count++;
            }
            if (DeletePointsID.Contains(FacePs[2]))
            {
                FaceP_is_DeleteP[2] = 1;
                DeleteP_Count++;
            }
            if (DeletePointsID.Contains(FacePs[3]))
            {
                FaceP_is_DeleteP[3] = 1;
                DeleteP_Count++;
            }
            #endregion

            // Rebuild Triangls on this Face
            if (DeleteP_Count == 0)
            {
                for (int j = 0; j < 6; j++)
                {
                    Triangles.Add(MC_Data.FaceTriangles[i * 6 + j]);
                }
            }
            else if (DeleteP_Count == 1)
            {
                #region Find Points
                // Find DeletePoint's ID on the Face
                int DeleteP_ID = -1;
                for (int j = 0; j < 4; j++)
                {
                    if (FaceP_is_DeleteP[j] == 1)
                    {
                        DeleteP_ID = j;
                        break;
                    }
                }

                // Find the previous and next Points and MidPoints of the DeletePoint on the Face
                int[] value = Find_prePoint_nexPoint_preMidPoint_nexMidPoint(FacePs, DeleteP_ID);
                int preP = value[0];
                int nexP = value[1];
                int preMidP = value[2];
                int nexMidP = value[3];

                // the Diagonal Point of the Delete Point on the Face
                int diaP;
                if (DeleteP_ID < 2) diaP = FacePs[DeleteP_ID + 2];
                else diaP = FacePs[DeleteP_ID - 2];
                #endregion

                int[] T0 = { preP, preMidP, diaP };
                int[] T1 = { preMidP, nexMidP, diaP };
                int[] T2 = { nexMidP, nexP, diaP };
                Triangles.AddRange(T0);
                Triangles.AddRange(T1);
                Triangles.AddRange(T2);
            }
            else if (DeleteP_Count == 2)
            {
                #region Find Points
                // Find DeletePoints' IDs on the Face
                int DP0ID = -1, DP1ID = -1;
                for (int j = 0; j < 4; j++)
                {
                    if (FaceP_is_DeleteP[j] == 1)
                    {
                        if (DP0ID == -1)
                        {
                            DP0ID = j;
                        }
                        else if (DP1ID == -1)
                        {
                            DP1ID = j;
                        }
                    }
                    if (DP0ID != -1 && DP1ID != -1) break;
                }
                if (DP0ID == 0 && DP1ID == 3)
                {
                    DP0ID = 3;
                    DP1ID = 0;
                }

                // Find the previous and next Points and MidPoints of the DeletePoints on the Face
                int[] value0 = Find_prePoint_nexPoint_preMidPoint_nexMidPoint(FacePs, DP0ID);
                int preP0 = value0[0];
                int nexP0 = value0[1];
                int preMidP0 = value0[2];
                int nexMidP0 = value0[3];

                int[] value1 = Find_prePoint_nexPoint_preMidPoint_nexMidPoint(FacePs, DP1ID);
                int preP1 = value1[0];
                int nexP1 = value1[1];
                int preMidP1 = value1[2];
                int nexMidP1 = value1[3];
                #endregion

                // if DeletePoints connect with each other
                if (DP1ID == DP0ID + 1 || DP0ID == 3 && DP1ID == 0)
                {
                    int[] T0 = { preP0, preMidP0, nexMidP1 };
                    int[] T1 = { nexMidP1, nexP1, preP0 };
                    Triangles.AddRange(T0);
                    Triangles.AddRange(T1);
                }
                else
                {
                    int[] T0 = { nexMidP1, preP0, preMidP0 };
                    int[] T1 = { nexMidP1, preMidP0, nexMidP0 };
                    int[] T2 = { nexMidP1, nexMidP0, preMidP1 };
                    int[] T3 = { preMidP1, nexMidP0, nexP0 };
                    Triangles.AddRange(T0);
                    Triangles.AddRange(T1);
                    Triangles.AddRange(T2);
                    Triangles.AddRange(T3);
                }
            }
            else if (DeleteP_Count == 3)
            {
                #region Find Points
                int notDeleteP_ID = -1;
                for (int j = 0; j < 4; j++)
                {
                    if (FaceP_is_DeleteP[j] == 0)
                    {
                        notDeleteP_ID = j;
                        break;
                    }
                }

                int[] value = Find_prePoint_nexPoint_preMidPoint_nexMidPoint(FacePs, notDeleteP_ID);
                int preMidP = value[2];
                int nexMidP = value[3];
                #endregion

                int[] T0 = { FacePs[notDeleteP_ID], nexMidP, preMidP };
                Triangles.AddRange(T0);
            }
        }

        return Triangles.ToArray();
    }



    /* ------------------------------ Fill Face Algorithm ------------------------------ */
    struct Basic_MidPointTriangle
    {
        public int T0, T1, T2;
        public int DeleteP_ID;
    }

    public int[] Fill_Faces(List<int> DeletePointsID, bool inverse)
    {
        List<int> Triangles = new();

        #region Detect if it is a special situations, and deal with it
        bool Is_Special_Situation = false;
        if (inverse)
        {
            if (DeletePointsID.Count == 3)
            {
                #region Detect how many DeletePoints are connect with each other
                bool conn0 = false, conn1 = false, conn2 = false;
                int connCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    // Three connect Point of this DeletePoint
                    int P0 = MC_Data.PconnPs[DeletePointsID[i] * 3];
                    int P1 = MC_Data.PconnPs[DeletePointsID[i] * 3 + 1];
                    int P2 = MC_Data.PconnPs[DeletePointsID[i] * 3 + 2];

                    // Two other DeletePoints
                    int otherDP0 = -1, otherDP1 = -1;
                    if (i == 0)
                    {
                        otherDP0 = DeletePointsID[1];
                        otherDP1 = DeletePointsID[2];
                    }
                    else if (i == 1)
                    {
                        otherDP0 = DeletePointsID[2];
                        otherDP1 = DeletePointsID[0];
                    }
                    else if (i == 2)
                    {
                        otherDP0 = DeletePointsID[0];
                        otherDP1 = DeletePointsID[1];
                    }

                    if (P0 == otherDP0 || P0 == otherDP1 || P1 == otherDP0 || P1 == otherDP1 || P2 == otherDP0 || P2 == otherDP1)
                    {
                        if (i == 0)
                        {
                            conn0 = true;
                            connCount++;
                        }
                        else if (i == 1)
                        {
                            conn1 = true;
                            connCount++;
                        }
                        else if (i == 2)
                        {
                            conn2 = true;
                            connCount++;
                        }
                    }
                }
                #endregion

                if (connCount == 0)
                {
                    Is_Special_Situation = true;

                    #region Situation 0: DeletePoints Count is 3 && none of them is connect with each other

                    #region FillFace 0

                    #region Find the Point that connect all 3 DeletePoints
                    int PointID = -1;
                    for(int i = 0; i < 8; i++)
                    {
                        if(MC_Data.PconnPs[i * 3] == DeletePointsID[0] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[1] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[2]
                        || MC_Data.PconnPs[i * 3] == DeletePointsID[0] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[2] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[1]
                        || MC_Data.PconnPs[i * 3] == DeletePointsID[1] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[0] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[2]
                        || MC_Data.PconnPs[i * 3] == DeletePointsID[1] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[2] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[0]
                        || MC_Data.PconnPs[i * 3] == DeletePointsID[2] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[0] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[1]
                        || MC_Data.PconnPs[i * 3] == DeletePointsID[2] && MC_Data.PconnPs[i * 3 + 1] == DeletePointsID[1] && MC_Data.PconnPs[i * 3 + 2] == DeletePointsID[0])
                        {
                            PointID = i;
                            break;
                        }
                    }
                    #endregion

                    // Add the Basic_MidPointTriangle of this Point to the FillFace
                    Triangles.Add(MC_Data.PconnMidPs[PointID * 3]);
                    Triangles.Add(MC_Data.PconnMidPs[PointID * 3 + 2]);
                    Triangles.Add(MC_Data.PconnMidPs[PointID * 3 + 1]);

                    #endregion

                    #region FillFace 1

                    #region Collect all the MidPoints in the FillFace1 in order
                    // Collect all the MidPoins on this FillFace
                    List<int> FF_MidP = new();

                    // Check the MidPoints that three DeletePoint connect
                    for(int i = 0; i < 3; i++)
                    {
                        int MP0 = MC_Data.PconnMidPs[DeletePointsID[i] * 3];
                        int MP1 = MC_Data.PconnMidPs[DeletePointsID[i] * 3 + 1];
                        int MP2 = MC_Data.PconnMidPs[DeletePointsID[i] * 3 + 2];

                        if (!Triangles.Contains(MP0) && !Triangles.Contains(MP1))
                        {
                            FF_MidP.Add(MP0);
                            FF_MidP.Add(MP1);
                        }
                        else if(!Triangles.Contains(MP1) && !Triangles.Contains(MP2))
                        {
                            FF_MidP.Add(MP1);
                            FF_MidP.Add(MP2);
                        }
                        else if(!Triangles.Contains(MP2) && !Triangles.Contains(MP0))
                        {
                            FF_MidP.Add(MP2);
                            FF_MidP.Add(MP0);
                        }
                    }

                    #region Reorder these MidPoints
                    List<int> tempFF_MidP = new();

                    // Add the first two MidPoints
                    tempFF_MidP.Add(FF_MidP[0]);
                    tempFF_MidP.Add(FF_MidP[1]);

                    // Keep adding FF_MidP[]'s MidPoints in order into tempFF_MidP[]
                    for (int i = 0; i < FF_MidP.Count - 2; i++)
                    {
                        // Find the Face that Last tempFF_MidP is on, and the Penult tempFF_MidP is not on
                        for (int j = 0; j < 6; j++)
                        {
                            bool Found_the_Face = false;

                            // Check all the MidPoints on this Face
                            for(int k = 0; k < 4; k++)
                            {
                                #region Get this MidPoint, previous MidPoint, next MidPoint
                                int thisMP = MC_Data.FaceMidPs[j * 4 + k];
                                int preMP = -1, nexMP = -1;
                                if (k == 0) preMP = MC_Data.FaceMidPs[j * 4 + 3];
                                else preMP = MC_Data.FaceMidPs[j * 4 + k - 1];
                                if (k == 3) nexMP = MC_Data.FaceMidPs[j * 4];
                                else nexMP = MC_Data.FaceMidPs[j * 4 + k + 1];
                                #endregion

                                if(thisMP == tempFF_MidP[^1] && preMP != tempFF_MidP[^2] && nexMP != tempFF_MidP[^2])
                                {
                                    //Add the next MidPoint in order
                                    if (FF_MidP.Contains(preMP) && !tempFF_MidP.Contains(preMP)) tempFF_MidP.Add(preMP);
                                    else if (FF_MidP.Contains(nexMP) && !tempFF_MidP.Contains(nexMP)) tempFF_MidP.Add(nexMP);

                                    Found_the_Face = true;
                                    break;
                                }
                            }

                            if (Found_the_Face) break;
                        }
                    }

                    FF_MidP = tempFF_MidP;
                    #endregion

                    #endregion

                    Triangles.Add(FF_MidP[0]);
                    Triangles.Add(FF_MidP[1]);
                    Triangles.Add(FF_MidP[2]);

                    Triangles.Add(FF_MidP[3]);
                    Triangles.Add(FF_MidP[4]);
                    Triangles.Add(FF_MidP[5]);

                    Triangles.Add(FF_MidP[5]);
                    Triangles.Add(FF_MidP[0]);
                    Triangles.Add(FF_MidP[2]);

                    Triangles.Add(FF_MidP[2]);
                    Triangles.Add(FF_MidP[3]);
                    Triangles.Add(FF_MidP[5]);

                    #endregion

                    #endregion
                }
                else if (connCount == 2)
                {
                    Is_Special_Situation = true;

                    #region Situation 1: DeletePoints Count is 3 && only two of them are connect

                    #region Find the two Points that connect with each other, and the Point that not connect
                    int conn_Point_ID0 = -1, conn_Point_ID1 = -1;
                    int not_conn_Point_ID = -1;

                    if (!conn0)
                    {
                        not_conn_Point_ID = DeletePointsID[0];
                        conn_Point_ID0 = DeletePointsID[1];
                        conn_Point_ID1 = DeletePointsID[2];
                    }
                    else if (!conn1)
                    {
                        not_conn_Point_ID = DeletePointsID[1];
                        conn_Point_ID0 = DeletePointsID[0];
                        conn_Point_ID1 = DeletePointsID[2];
                    }
                    else if (!conn2)
                    {
                        not_conn_Point_ID = DeletePointsID[2];
                        conn_Point_ID0 = DeletePointsID[0];
                        conn_Point_ID1 = DeletePointsID[1];
                    }
                    #endregion

                    #region Collect all the MidPoins on this FillFace
                    List<int> FF_MidP = new();

                    #region Find two basic MidPointTriangles of two Points that connect with each other, and Remove the same MidPoint
                    // Three MidPoints of basic MidPointTriangles
                    int[] TPs0 = { MC_Data.PconnMidPs[conn_Point_ID0 * 3], MC_Data.PconnMidPs[conn_Point_ID0 * 3 + 1], MC_Data.PconnMidPs[conn_Point_ID0 * 3 + 2] };
                    int[] TPs1 = { MC_Data.PconnMidPs[conn_Point_ID1 * 3], MC_Data.PconnMidPs[conn_Point_ID1 * 3 + 1], MC_Data.PconnMidPs[conn_Point_ID1 * 3 + 2] };

                    for(int i = 0; i < 3; i++)
                    {
                        // Detect if this MidPoint in TPs0 is also in TPs1
                        bool Same = false;

                        for(int j = 0; j < 3; j++)
                        {
                            if(TPs0[i] == TPs1[j])
                            {
                                Same = true;

                                // Add other two MidPoints in TPs1 into the FillFace
                                if (j == 0)
                                {
                                    FF_MidP.Add(TPs1[1]);
                                    FF_MidP.Add(TPs1[2]);
                                }
                                else if (j == 1)
                                {
                                    FF_MidP.Add(TPs1[2]);
                                    FF_MidP.Add(TPs1[0]);
                                }
                                else if (j == 2)
                                {
                                    FF_MidP.Add(TPs1[0]);
                                    FF_MidP.Add(TPs1[1]);
                                }

                                break;
                            }
                        }

                        if (!Same)
                        {
                            FF_MidP.Add(TPs0[i]);
                        }
                    }
                    #endregion

                    #region Find the Three MidPoints that the not connected Point connect
                    int NCP_MP0 = MC_Data.PconnMidPs[not_conn_Point_ID * 3];
                    int NCP_MP1 = MC_Data.PconnMidPs[not_conn_Point_ID * 3 + 1];
                    int NCP_MP2 = MC_Data.PconnMidPs[not_conn_Point_ID * 3 + 2];
                    FF_MidP.Add(NCP_MP0);
                    FF_MidP.Add(NCP_MP1);
                    FF_MidP.Add(NCP_MP2);
                    #endregion

                    #endregion

                    // Find the Face that all the MidPoints on it are in the FillFace
                    for (int i = 0; i < 6; i++)
                    {
                        bool All_MP_in_FF = true;
                        
                        // Check four MidPoints on this Face
                        for(int j = 0; j < 4; j++)
                        {
                            if(!FF_MidP.Contains(MC_Data.FaceMidPs[i * 4 + j]))
                            {
                                All_MP_in_FF = false;
                                break;
                            }
                        }

                        if (All_MP_in_FF)
                        {
                            #region Find two NCP_MPs on this Face, and one NCP_MP not on this Face
                            bool onFace0 = false, onFace1 = false, onFace2 = false;
                            int onFace_MP0 = -1, onFace_MP1 = -1, N_onFace_MP = -1;

                            for(int j = 0; j < 4; j++)
                            {
                                if (MC_Data.FaceMidPs[i * 4 + j] == NCP_MP0) onFace0 = true;
                                else if (MC_Data.FaceMidPs[i * 4 + j] == NCP_MP1) onFace1 = true;
                                else if (MC_Data.FaceMidPs[i * 4 + j] == NCP_MP2) onFace2 = true;
                            }

                            // Three NCP_MPS connect order: onFace_MP0 --> N_onFace_MP --> onFace_MP1
                            if (!onFace0)
                            {
                                N_onFace_MP = NCP_MP0;
                                onFace_MP0 = NCP_MP2;
                                onFace_MP1 = NCP_MP1;
                            }
                            else if (!onFace1)
                            {
                                N_onFace_MP = NCP_MP1;
                                onFace_MP0 = NCP_MP0;
                                onFace_MP1 = NCP_MP2;
                            }
                            else if (!onFace2)
                            {
                                N_onFace_MP = NCP_MP2;
                                onFace_MP0 = NCP_MP1;
                                onFace_MP1 = NCP_MP0;
                            }
                            #endregion

                            #region Find the two other MidPoints that is on the side of two onFace_MP
                            int sideMP0 = -1, sideMP1 = -1;

                            for(int j = 0; j < 4; j++)
                            {
                                int ID = -1, preID = -1, nexID = -1;
                                ID = i * 4 + j;
                                if (j == 0) preID = i * 4 + 3;
                                else preID = i * 4 + j - 1;
                                if (j == 3) nexID = i * 4;
                                else nexID = i * 4 + j + 1;

                                if (MC_Data.FaceMidPs[ID] == onFace_MP0)
                                {
                                    if (MC_Data.FaceMidPs[preID] != onFace_MP1) sideMP0 = MC_Data.FaceMidPs[preID];
                                    else if (MC_Data.FaceMidPs[nexID] != onFace_MP1) sideMP0 = MC_Data.FaceMidPs[nexID];
                                }
                                else if (MC_Data.FaceMidPs[ID] == onFace_MP1)
                                {
                                    if (MC_Data.FaceMidPs[preID] != onFace_MP0) sideMP1 = MC_Data.FaceMidPs[preID];
                                    else if (MC_Data.FaceMidPs[nexID] != onFace_MP0) sideMP1 = MC_Data.FaceMidPs[nexID];
                                }
                            }
                            #endregion
                            
                            Triangles.Add(N_onFace_MP);
                            Triangles.Add(sideMP0);
                            Triangles.Add(onFace_MP0);
                            
                            Triangles.Add(N_onFace_MP);
                            Triangles.Add(onFace_MP1);
                            Triangles.Add(sideMP1);

                            #region Find the other Face that sideMPs are on, and find the FF_MidP that is on the opposite Edge of the sideMP;
                            int opp_MP0 = -1, opp_MP1 = -1;
                            for(int j = 0; j < 6; j++)
                            {
                                // Make sure this is not the previous Face
                                if(j != i)
                                {
                                    for(int k = 0; k < 4; k++)
                                    {
                                        int ID = j * 4 + k;
                                        int oppID = j * 4 + k + 2;
                                        if(k > 1) oppID -= 4;

                                        if (MC_Data.FaceMidPs[ID] == sideMP0)
                                        {
                                            opp_MP0 = MC_Data.FaceMidPs[oppID];
                                            break;
                                        }
                                        else if (MC_Data.FaceMidPs[ID] == sideMP1)
                                        {
                                            opp_MP1 = MC_Data.FaceMidPs[oppID];
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion

                            Triangles.Add(N_onFace_MP);
                            Triangles.Add(opp_MP0);
                            Triangles.Add(sideMP0);
                            
                            Triangles.Add(N_onFace_MP);
                            Triangles.Add(sideMP1);
                            Triangles.Add(opp_MP1);
                            
                            Triangles.Add(N_onFace_MP);
                            Triangles.Add(opp_MP1);
                            Triangles.Add(opp_MP0);

                            break;
                        }
                    }

                    #endregion
                }
            }

            #region Situation 2: DeletePoints Count is 2 && they are on one Face && they are not connect
            else if (DeletePointsID.Count == 2)
            {
                // Detect all Faces
                for(int i = 0; i < 6; i++)
                {
                    if(MC_Data.FacePs[i * 4] == DeletePointsID[0] && MC_Data.FacePs[i * 4 + 2] == DeletePointsID[1]
                    || MC_Data.FacePs[i * 4] == DeletePointsID[1] && MC_Data.FacePs[i * 4 + 2] == DeletePointsID[0]
                    || MC_Data.FacePs[i * 4 + 1] == DeletePointsID[0] && MC_Data.FacePs[i * 4 + 3] == DeletePointsID[1]
                    || MC_Data.FacePs[i * 4 + 1] == DeletePointsID[1] && MC_Data.FacePs[i * 4 + 3] == DeletePointsID[0])
                    {
                        Is_Special_Situation = true;

                        // Get an FF_MidP that is not on the Face that all MidPoints on it are FF_MidP, this will be the MeetingPoint of final Triangles
                        int MeetingPoint = -1;

                        #region Collect all the MidPoins on this FillFace in order
                        List<int> FF_MidP = new();

                        // Add two basic MidPointTriangles of the DeletePoints to the FillFace
                        int[] BMPT0 = { MC_Data.PconnMidPs[DeletePointsID[0] * 3], MC_Data.PconnMidPs[DeletePointsID[0] * 3 + 1], MC_Data.PconnMidPs[DeletePointsID[0] * 3 + 2] };
                        int[] BMPT1 = { MC_Data.PconnMidPs[DeletePointsID[1] * 3], MC_Data.PconnMidPs[DeletePointsID[1] * 3 + 1], MC_Data.PconnMidPs[DeletePointsID[1] * 3 + 2] };
                        FF_MidP.AddRange(BMPT0);
                        FF_MidP.AddRange(BMPT1);

                        #region Reorder these MidPoints
                        List<int> tempFF_MidP = new();

                        // Find the Face that all MidPoints on it are FF_MidP
                        for (int j = 0; j < 6; j++)
                        {
                            // Four MidPoints on this Face
                            int FMP0 = MC_Data.FaceMidPs[j * 4];
                            int FMP1 = MC_Data.FaceMidPs[j * 4 + 1];
                            int FMP2 = MC_Data.FaceMidPs[j * 4 + 2];
                            int FMP3 = MC_Data.FaceMidPs[j * 4 + 3];

                            if (FF_MidP.Contains(FMP0) && FF_MidP.Contains(FMP1) && FF_MidP.Contains(FMP2) && FF_MidP.Contains(FMP3))
                            {
                                #region Find two MidPoints that from diffrent basic MidPointTriangles
                                int fromDifTri_MP0, fromDifTri_MP1;
                                int P0_fromT_ID = -1, P1_fromT_ID = -1;
                                for(int k = 0; k < 3; k++)
                                {
                                    if (BMPT0[k] == FMP0) P0_fromT_ID = 0;
                                    else if (BMPT0[k] == FMP1) P1_fromT_ID = 0;
                                }
                                for(int k = 0; k < 3; k++)
                                {
                                    if (BMPT1[k] == FMP0) P0_fromT_ID = 1;
                                    if (BMPT1[k] == FMP1) P1_fromT_ID = 1;
                                }

                                // if two MidPoints are from same basic MidPointTriangle
                                if (P0_fromT_ID == P1_fromT_ID)
                                {
                                    fromDifTri_MP0 = FMP1;
                                    fromDifTri_MP1 = FMP2;
                                }
                                // if two MidPoints are from different basic MidPointTriangles
                                else
                                {
                                    fromDifTri_MP0 = FMP0;
                                    fromDifTri_MP1 = FMP1;
                                }
                                #endregion

                                #region Add two MidPoints from differnt basic MidPointTriangles to FillFace in order
                                // if fromDifTri_MP0 is in BMPT0
                                for (int k = 0; k < 3; k++)
                                {
                                    int nexID = k + 1;
                                    if (nexID > 2) nexID = 0;

                                    if(BMPT0[k] == fromDifTri_MP0)
                                    {
                                        // if the next basic MidPointTriangle MidPoint is also in this Face
                                        if (BMPT0[nexID] == FMP0 || BMPT0[nexID] == FMP3 || BMPT0[nexID] == FMP2 || BMPT0[nexID] == FMP3)
                                        {
                                            tempFF_MidP.Add(fromDifTri_MP0);
                                            tempFF_MidP.Add(fromDifTri_MP1);
                                        }
                                        else
                                        {
                                            tempFF_MidP.Add(fromDifTri_MP1);
                                            tempFF_MidP.Add(fromDifTri_MP0);
                                        }

                                        break;
                                    }
                                }

                                // if fromDifTri_MP0 is in BMPT1
                                for (int k = 0; k < 3; k++)
                                {
                                    int nexID = k + 1;
                                    if (nexID > 2) nexID = 0;

                                    if(BMPT1[k] == fromDifTri_MP0)
                                    {
                                        // if the next basic MidPointTriangle MidPoint is also in this Face
                                        if (BMPT1[nexID] == FMP0 || BMPT1[nexID] == FMP3 || BMPT1[nexID] == FMP2 || BMPT1[nexID] == FMP3)
                                        {
                                            tempFF_MidP.Add(fromDifTri_MP0);
                                            tempFF_MidP.Add(fromDifTri_MP1);
                                        }
                                        else
                                        {
                                            tempFF_MidP.Add(fromDifTri_MP1);
                                            tempFF_MidP.Add(fromDifTri_MP0);
                                        }

                                        break;
                                    }
                                }
                                #endregion

                                break;
                            }
                        }

                        // Keep adding FF_MidP[]'s MidPoints in order into tempFF_MidP[]
                        for (int j = 0; j < FF_MidP.Count - 2; j++)
                        {
                            // Find the Face other than the Face that all MidPoints on it are FF_MidP, that Last tempFF_MidP is on, and the Penult tempFF_MidP is not on
                            for (int k = 0; k < 6; k++)
                            {
                                bool Found_the_Face = false;

                                // Check all the MidPoints on this Face
                                for (int g = 0; g < 4; g++)
                                {
                                    #region Get this MidPoint, previous MidPoint, next MidPoint
                                    int thisMP = MC_Data.FaceMidPs[k * 4 + g];
                                    int preMP = -1, nexMP = -1;
                                    if (g == 0) preMP = MC_Data.FaceMidPs[k * 4 + 3];
                                    else preMP = MC_Data.FaceMidPs[k * 4 + g - 1];
                                    if (g == 3) nexMP = MC_Data.FaceMidPs[k * 4];
                                    else nexMP = MC_Data.FaceMidPs[k * 4 + g + 1];
                                    #endregion

                                    if (thisMP == tempFF_MidP[^1] && preMP != tempFF_MidP[^2] && nexMP != tempFF_MidP[^2])
                                    {
                                        #region Find the FF_MidP that is not on the Face that all MidPoints on it are FF_MidP\
                                        if(tempFF_MidP.Count == 2)
                                        {
                                            if (FF_MidP.Contains(preMP)) MeetingPoint = preMP;
                                            else if (FF_MidP.Contains(nexMP)) MeetingPoint = nexMP;
                                        }
                                        #endregion

                                        //Add the next MidPoint in order
                                        if (FF_MidP.Contains(preMP) && !tempFF_MidP.Contains(preMP)) tempFF_MidP.Add(preMP);
                                        else if (FF_MidP.Contains(nexMP) && !tempFF_MidP.Contains(nexMP)) tempFF_MidP.Add(nexMP);

                                        Found_the_Face = true;
                                        break;
                                    }
                                }

                                if (Found_the_Face) break;
                            }
                        }

                        FF_MidP = tempFF_MidP;
                        #endregion

                        #endregion

                        #region Create Triangles in order to the MeetingPoint
                        for(int j = 0; j < FF_MidP.Count; j++)
                        {
                            // First Two points of this Triangle
                            int T0 = -1, T1 = -1;
                            
                            if(j == FF_MidP.Count - 1)
                            {
                                T0 = FF_MidP[j];
                                T1 = FF_MidP[0];
                            }
                            else
                            {
                                T0 = FF_MidP[j];
                                T1 = FF_MidP[j + 1];
                            }

                            if (T0 != MeetingPoint && T1 != MeetingPoint)
                            {
                                Triangles.Add(T0);
                                Triangles.Add(T1);
                                Triangles.Add(MeetingPoint);
                            }
                        }
                        #endregion

                        break;
                    }
                }
            }
            #endregion
        }


        #endregion

        if (!Is_Special_Situation)
        {
            #region Collect all Basic_MidPointTriangle
            List<Basic_MidPointTriangle> Basic_MPTs = new();
            for (int i = 0; i < DeletePointsID.Count; i++)
            {
                Basic_MidPointTriangle Basic_MPT = new();
                Basic_MPT.T0 = MC_Data.PconnMidPs[DeletePointsID[i] * 3];
                Basic_MPT.T1 = MC_Data.PconnMidPs[DeletePointsID[i] * 3 + 1];
                Basic_MPT.T2 = MC_Data.PconnMidPs[DeletePointsID[i] * 3 + 2];

                Basic_MPT.DeleteP_ID = DeletePointsID[i];

                Basic_MPTs.Add(Basic_MPT);
            }
            #endregion

            int Basic_MPT_DealCount = 0;
            int Basic_MPTs_TotalCount = Basic_MPTs.Count;

            // Detect if we have dealt with all the MidPointTriangles
            while (Basic_MPT_DealCount < Basic_MPTs_TotalCount)
            {
                // Collect all the MidPoins on this FillFace
                List<int> FF_MidP = new();
                // Collect all this FillFace related DeletePoins
                List<int> FF_DeleteP_ID = new();

                #region Add the first MidPointTriangle into FillFace
                // Always get the first MidPointTriangle in Basic_MPTs[]
                FF_MidP.Add(Basic_MPTs[0].T0);
                FF_MidP.Add(Basic_MPTs[0].T1);
                FF_MidP.Add(Basic_MPTs[0].T2);
                FF_DeleteP_ID.Add(Basic_MPTs[0].DeleteP_ID);

                // Remove this first MidPointTriangle in Basic_MPTs[]
                Basic_MPTs.RemoveAt(0);
                #endregion

                #region Continue merging connected MidPointTriangles with this FillFace until the end
                // Detect if there is a MidPointTriangle in Basic_MPTs[] that can merge with this FillFace
                for (int i = 0; i < FF_MidP.Count; i++)
                {
                    // Only merge one MidPointTriangle at one time
                    for (int j = 0; j < Basic_MPTs.Count; j++)
                    {
                        // Three Points of the MidPointTriangle
                        int Basic_MPT0 = Basic_MPTs[j].T0;
                        int Basic_MPT1 = Basic_MPTs[j].T1;
                        int Basic_MPT2 = Basic_MPTs[j].T2;

                        // Detect if this MidPointTriangle connects with this FillFace
                        if (FF_MidP[i] == Basic_MPT0 || FF_MidP[i] == Basic_MPT1 || FF_MidP[i] == Basic_MPT2)
                        {
                            // only keep the MidPoins that is not connect with this FillFace
                            List<int> keepMidP = new();
                            List<int> notkeepMidP = new();

                            #region Find two other MidPoints
                            int otherBasic_MPT0 = -1, otherBasic_MPT1 = -1;
                            if (FF_MidP[i] == Basic_MPT0)
                            {
                                otherBasic_MPT0 = Basic_MPT1; otherBasic_MPT1 = Basic_MPT2;
                            }
                            else if (FF_MidP[i] == Basic_MPT1)
                            {
                                otherBasic_MPT0 = Basic_MPT2; otherBasic_MPT1 = Basic_MPT0;
                            }
                            else if (FF_MidP[i] == Basic_MPT2)
                            {
                                otherBasic_MPT0 = Basic_MPT0; otherBasic_MPT1 = Basic_MPT1;
                            }
                            #endregion

                            // Detect if these two other MidPoints connects with this FillFace
                            bool conn0 = false, conn1 = false;
                            for (int k = 0; k < FF_MidP.Count; k++)
                            {
                                if (FF_MidP[k] == otherBasic_MPT0) conn0 = true;
                                if (FF_MidP[k] == otherBasic_MPT1) conn1 = true;
                            }

                            if (!conn0) keepMidP.Add(otherBasic_MPT0);
                            if (!conn1) keepMidP.Add(otherBasic_MPT1);

                            if (!keepMidP.Contains(Basic_MPT0)) notkeepMidP.Add(Basic_MPT0);
                            if (!keepMidP.Contains(Basic_MPT1)) notkeepMidP.Add(Basic_MPT1);
                            if (!keepMidP.Contains(Basic_MPT2)) notkeepMidP.Add(Basic_MPT2);

                            #region Merge this MidPointTriangle into this FillFace
                            List<int> tempFF_MidP = new();
                            bool Basic_MPTmerged = false;

                            for (int k = 0; k < FF_MidP.Count; k++)
                            {
                                if (notkeepMidP.Contains(FF_MidP[k]))
                                {
                                    if (!Basic_MPTmerged)
                                    {
                                        // Insert the keptMidPoint on this MidPointTriangle in order into the new FillFace
                                        tempFF_MidP.AddRange(keepMidP.ToArray());
                                        Basic_MPTmerged = true;
                                    }
                                }
                                else
                                {
                                    // Add this FillFace_MidPoint to the new FillFace
                                    tempFF_MidP.Add(FF_MidP[k]);
                                }
                            }

                            FF_MidP = tempFF_MidP;
                            FF_DeleteP_ID.Add(Basic_MPTs[j].DeleteP_ID);

                            Basic_MPTs.RemoveAt(j);
                            i = -1;
                            #endregion

                            break;
                        }
                    }
                }
                #endregion

                #region Deal with this FillFace based on how many DeletePoints this FillFace related
                if (FF_DeleteP_ID.Count == 1)
                {
                    // Build Triangles
                    Triangles.AddRange(FF_MidP.ToArray());
                }
                else if (FF_DeleteP_ID.Count == 2)
                {
                    // Get the connected MidPoints of the first FillFace_related_DeletedPoint
                    int[] DP0_connMidPs = { MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3], MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3 + 1], MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3 + 2] };

                    for (int i = 0; i < FF_MidP.Count; i++)
                    {
                        #region two MidPoints in order on the FillFace
                        int FF0 = FF_MidP[i];
                        int FF1;
                        if (i < FF_MidP.Count - 1) FF1 = FF_MidP[i + 1];
                        else FF1 = FF_MidP[0];
                        #endregion

                        // Detect if these two MidPoints are common between FillFace and the DeletedPoint
                        if (FF0 == DP0_connMidPs[0] && FF1 == DP0_connMidPs[1]
                            || FF0 == DP0_connMidPs[1] && FF1 == DP0_connMidPs[2]
                            || FF0 == DP0_connMidPs[2] && FF1 == DP0_connMidPs[0])
                        {
                            #region Reorder MidPoints on the FillFace
                            List<int> tempFF = new();
                            for (int j = 0; j < FF_MidP.Count; j++)
                            {
                                if (i + j < FF_MidP.Count) tempFF.Add(FF_MidP[i + j]);
                                else tempFF.Add(FF_MidP[i + j - FF_MidP.Count]);
                            }
                            FF_MidP = tempFF;
                            #endregion

                            break;
                        }
                    }

                    // Build Triangles
                    for (int i = 0; i < FF_MidP.Count - 1; i += 2)
                    {
                        Triangles.Add(FF_MidP[i]);
                        Triangles.Add(FF_MidP[i + 1]);
                        if (i + 2 < FF_MidP.Count) Triangles.Add(FF_MidP[i + 2]);
                        else Triangles.Add(FF_MidP[i + 2 - FF_MidP.Count]);
                    }
                }
                else if (FF_DeleteP_ID.Count == 3)
                {
                    // Get the connected MidPoints of three FillFace_related_DeletedPoint
                    int[] DP0_connMidPs = { MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3], MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3 + 1], MC_Data.PconnMidPs[FF_DeleteP_ID[0] * 3 + 2] };
                    int[] DP1_connMidPs = { MC_Data.PconnMidPs[FF_DeleteP_ID[1] * 3], MC_Data.PconnMidPs[FF_DeleteP_ID[1] * 3 + 1], MC_Data.PconnMidPs[FF_DeleteP_ID[1] * 3 + 2] };
                    int[] DP2_connMidPs = { MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3], MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3 + 1], MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3 + 2] };

                    List<int> commMidPs0 = new();
                    List<int> commMidPs1 = new();
                    List<int> commMidPs2 = new();

                    #region Find two DeletePoints that have two common MidPoints with the FillFace
                    List<int> commMidP2_DP = new();

                    for (int i = 0; i < FF_MidP.Count; i++)
                    {
                        #region two MidPoints in order on the FillFace
                        int FF0 = FF_MidP[i];
                        int FF1;
                        if (i < FF_MidP.Count - 1) FF1 = FF_MidP[i + 1];
                        else FF1 = FF_MidP[0];
                        #endregion

                        if (commMidPs0.Count == 0)
                        {
                            if (FF0 == DP0_connMidPs[0] && FF1 == DP0_connMidPs[1]
                                || FF0 == DP0_connMidPs[1] && FF1 == DP0_connMidPs[2]
                                || FF0 == DP0_connMidPs[2] && FF1 == DP0_connMidPs[0])
                            {
                                commMidPs0.Add(FF0);
                                commMidPs0.Add(FF1);
                                commMidP2_DP.Add(0);
                            }
                        }
                        if (commMidPs1.Count == 0)
                        {
                            if (FF0 == DP1_connMidPs[0] && FF1 == DP1_connMidPs[1]
                                || FF0 == DP1_connMidPs[1] && FF1 == DP1_connMidPs[2]
                                || FF0 == DP1_connMidPs[2] && FF1 == DP1_connMidPs[0])
                            {
                                commMidPs1.Add(FF0);
                                commMidPs1.Add(FF1);
                                commMidP2_DP.Add(1);
                            }
                        }
                        if (commMidPs2.Count == 0)
                        {
                            if (FF0 == DP2_connMidPs[0] && FF1 == DP2_connMidPs[1]
                                || FF0 == DP2_connMidPs[1] && FF1 == DP2_connMidPs[2]
                                || FF0 == DP2_connMidPs[2] && FF1 == DP2_connMidPs[0])
                            {
                                commMidPs2.Add(FF0);
                                commMidPs2.Add(FF1);
                                commMidP2_DP.Add(2);
                            }
                        }

                        if (commMidP2_DP.Count == 2) break;
                    }
                    #endregion

                    #region Build two Triangles with these common MidPoints
                    if (commMidP2_DP.Contains(0) && commMidP2_DP.Contains(1))
                    {
                        Triangles.AddRange(commMidPs0.ToArray());
                        Triangles.Add(commMidPs1[0]);
                        Triangles.AddRange(commMidPs1.ToArray());
                        Triangles.Add(commMidPs0[0]);
                    }
                    else if (commMidP2_DP.Contains(1) && commMidP2_DP.Contains(2))
                    {
                        Triangles.AddRange(commMidPs1.ToArray());
                        Triangles.Add(commMidPs2[0]);
                        Triangles.AddRange(commMidPs2.ToArray());
                        Triangles.Add(commMidPs1[0]);
                    }
                    else if (commMidP2_DP.Contains(2) && commMidP2_DP.Contains(0))
                    {
                        Triangles.AddRange(commMidPs2.ToArray());
                        Triangles.Add(commMidPs0[0]);
                        Triangles.AddRange(commMidPs0.ToArray());
                        Triangles.Add(commMidPs2[0]);
                    }
                    #endregion

                    #region Build the rest one Triangle
                    if (!commMidP2_DP.Contains(0))
                    {
                        for (int i = 0; i < FF_MidP.Count; i++)
                        {
                            if (FF_MidP[i] == DP0_connMidPs[0] || FF_MidP[i] == DP0_connMidPs[1] || FF_MidP[i] == DP0_connMidPs[2])
                            {
                                // Build Triangle
                                if (i > 0) Triangles.Add(FF_MidP[i - 1]);
                                else Triangles.Add(FF_MidP[FF_MidP.Count - 1]);
                                Triangles.Add(FF_MidP[i]);
                                if (i < FF_MidP.Count - 1) Triangles.Add(FF_MidP[i + 1]);
                                else Triangles.Add(FF_MidP[0]);
                            }
                        }
                    }
                    else if (!commMidP2_DP.Contains(1))
                    {
                        for (int i = 0; i < FF_MidP.Count; i++)
                        {
                            if (FF_MidP[i] == DP1_connMidPs[0] || FF_MidP[i] == DP1_connMidPs[1] || FF_MidP[i] == DP1_connMidPs[2])
                            {
                                // Build Triangle
                                if (i > 0) Triangles.Add(FF_MidP[i - 1]);
                                else Triangles.Add(FF_MidP[FF_MidP.Count - 1]);
                                Triangles.Add(FF_MidP[i]);
                                if (i < FF_MidP.Count - 1) Triangles.Add(FF_MidP[i + 1]);
                                else Triangles.Add(FF_MidP[0]);
                            }
                        }
                    }
                    else if (!commMidP2_DP.Contains(2))
                    {
                        for (int i = 0; i < FF_MidP.Count; i++)
                        {
                            if (FF_MidP[i] == DP2_connMidPs[0] || FF_MidP[i] == DP2_connMidPs[1] || FF_MidP[i] == DP2_connMidPs[2])
                            {
                                // Build Triangle
                                if (i > 0) Triangles.Add(FF_MidP[i - 1]);
                                else Triangles.Add(FF_MidP[FF_MidP.Count - 1]);
                                Triangles.Add(FF_MidP[i]);
                                if (i < FF_MidP.Count - 1) Triangles.Add(FF_MidP[i + 1]);
                                else Triangles.Add(FF_MidP[0]);
                            }
                        }
                    }
                    #endregion
                }
                else if (FF_DeleteP_ID.Count == 4)
                {
                    #region Situation 0 : all DeletePoints is on one Face
                    if (Triangles.Count == 0)
                    {
                        for (int i = 0; i < MC_Data.FacePs.Length - 3; i += 4)
                        {
                            List<int> FacePs = new();
                            FacePs.Add(MC_Data.FacePs[i]);
                            FacePs.Add(MC_Data.FacePs[i + 1]);
                            FacePs.Add(MC_Data.FacePs[i + 2]);
                            FacePs.Add(MC_Data.FacePs[i + 3]);

                            // Detect if all DeletePoints is on one Face
                            if (FacePs.Contains(FF_DeleteP_ID[0]) && FacePs.Contains(FF_DeleteP_ID[1]) && FacePs.Contains(FF_DeleteP_ID[2]) && FacePs.Contains(FF_DeleteP_ID[3]))
                            {
                                // Build Triangles
                                for (int j = 0; j < FF_MidP.Count - 1; j += 2)
                                {
                                    Triangles.Add(FF_MidP[j]);
                                    if (j + 1 < FF_MidP.Count) Triangles.Add(FF_MidP[j + 1]);
                                    else Triangles.Add(FF_MidP[j + 1 - FF_MidP.Count]);
                                    if (j + 2 < FF_MidP.Count) Triangles.Add(FF_MidP[j + 2]);
                                    else Triangles.Add(FF_MidP[j + 2 - FF_MidP.Count]);
                                }

                                break;
                            }
                        }
                    }
                    #endregion

                    #region Situation 1 : three DeletePoints connect to one same other DeletePoint
                    if (Triangles.Count == 0)
                    {
                        #region Collect all the connectedPoints of each DeletePoint
                        List<int> connP0 = new();
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3]);
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3 + 1]);
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3 + 2]);
                        List<int> connP1 = new();
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3]);
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3 + 1]);
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3 + 2]);
                        List<int> connP2 = new();
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3]);
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3 + 1]);
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3 + 2]);
                        List<int> connP3 = new();
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3]);
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3 + 1]);
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3 + 2]);
                        #endregion

                        // Detect if three DeletePoints connect to one same other DeletePoint
                        int centerDP = -1;
                        if (connP1.Contains(FF_DeleteP_ID[0]) && connP2.Contains(FF_DeleteP_ID[0]) && connP3.Contains(FF_DeleteP_ID[0])) centerDP = 0;
                        else if (connP0.Contains(FF_DeleteP_ID[1]) && connP2.Contains(FF_DeleteP_ID[1]) && connP3.Contains(FF_DeleteP_ID[1])) centerDP = 1;
                        else if (connP0.Contains(FF_DeleteP_ID[2]) && connP1.Contains(FF_DeleteP_ID[2]) && connP3.Contains(FF_DeleteP_ID[2])) centerDP = 2;
                        else if (connP0.Contains(FF_DeleteP_ID[3]) && connP1.Contains(FF_DeleteP_ID[3]) && connP2.Contains(FF_DeleteP_ID[3])) centerDP = 3;

                        if (centerDP != -1)
                        {
                            Triangles.Add(FF_MidP[0]);
                            Triangles.Add(FF_MidP[1]);
                            Triangles.Add(FF_MidP[2]);

                            Triangles.Add(FF_MidP[3]);
                            Triangles.Add(FF_MidP[4]);
                            Triangles.Add(FF_MidP[5]);

                            Triangles.Add(FF_MidP[5]);
                            Triangles.Add(FF_MidP[0]);
                            Triangles.Add(FF_MidP[2]);

                            Triangles.Add(FF_MidP[2]);
                            Triangles.Add(FF_MidP[3]);
                            Triangles.Add(FF_MidP[5]);
                        }
                    }

                    #endregion

                    #region Situation 2 : all DeletePoints are in a line 
                    if (Triangles.Count == 0)
                    {
                        #region Find the two Long Mids of this FillFace
                        int[] LE0 = { -1, -1 };
                        int[] LE1 = { -1, -1 };

                        for (int i = 0; i < FF_MidP.Count; i++)
                        {
                            #region two MidPoints in order on the FillFace
                            int FF0 = FF_MidP[i];
                            int FF1 = -1;
                            if (i < FF_MidP.Count - 1) FF1 = FF_MidP[i + 1];
                            else FF1 = FF_MidP[0];
                            #endregion

                            #region detect if these two MidPoints are on the opposite Edge of the Face
                            for (int j = 0; j < 6; j++)
                            {
                                int[] FMidPs = { MC_Data.FaceMidPs[j * 4], MC_Data.FaceMidPs[j * 4 + 1], MC_Data.FaceMidPs[j * 4 + 2], MC_Data.FaceMidPs[j * 4 + 3] };

                                if (FF0 == FMidPs[0] && FF1 == FMidPs[2] || FF1 == FMidPs[0] && FF0 == FMidPs[2]
                                    || FF0 == FMidPs[1] && FF1 == FMidPs[3] || FF1 == FMidPs[1] && FF0 == FMidPs[3])
                                {
                                    if (LE0[0] == -1)
                                    {
                                        LE0[0] = FF0;
                                        LE0[1] = FF1;
                                    }
                                    else if (LE1[0] == -1)
                                    {
                                        LE1[0] = FF0;
                                        LE1[1] = FF1;
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region Rearrange FillFace related DeletePoints in order in a line

                        #region Collect all the connectedPoints of each DeletePoint
                        List<int> connP0 = new();
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3]);
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3 + 1]);
                        connP0.Add(MC_Data.PconnPs[FF_DeleteP_ID[0] * 3 + 2]);
                        List<int> connP1 = new();
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3]);
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3 + 1]);
                        connP1.Add(MC_Data.PconnPs[FF_DeleteP_ID[1] * 3 + 2]);
                        List<int> connP2 = new();
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3]);
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3 + 1]);
                        connP2.Add(MC_Data.PconnPs[FF_DeleteP_ID[2] * 3 + 2]);
                        List<int> connP3 = new();
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3]);
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3 + 1]);
                        connP3.Add(MC_Data.PconnPs[FF_DeleteP_ID[3] * 3 + 2]);
                        #endregion

                        #region Find out how many other DeletedPoints does each DeletePoint connect
                        int connDeleteP_Count0 = 0;
                        int connDeleteP_Count1 = 0;
                        int connDeleteP_Count2 = 0;
                        int connDeleteP_Count3 = 0;

                        for (int i = 0; i < FF_DeleteP_ID.Count; i++)
                        {
                            if (connP0.Contains(FF_DeleteP_ID[i])) connDeleteP_Count0++;
                            if (connP1.Contains(FF_DeleteP_ID[i])) connDeleteP_Count1++;
                            if (connP2.Contains(FF_DeleteP_ID[i])) connDeleteP_Count2++;
                            if (connP3.Contains(FF_DeleteP_ID[i])) connDeleteP_Count3++;
                        }
                        #endregion

                        #region Find the two DeletedPoints that only connect one other DeletedPoint
                        int DPconn1DP0 = -1, DPconn1DP1 = -1;
                        if (connDeleteP_Count0 == 1)
                        {
                            if (DPconn1DP0 == -1) DPconn1DP0 = FF_DeleteP_ID[0];
                            else if (DPconn1DP1 == -1) DPconn1DP1 = FF_DeleteP_ID[0];
                        }
                        if (connDeleteP_Count1 == 1)
                        {
                            if (DPconn1DP0 == -1) DPconn1DP0 = FF_DeleteP_ID[1];
                            else if (DPconn1DP1 == -1) DPconn1DP1 = FF_DeleteP_ID[1];
                        }
                        if (connDeleteP_Count2 == 1)
                        {
                            if (DPconn1DP0 == -1) DPconn1DP0 = FF_DeleteP_ID[2];
                            else if (DPconn1DP1 == -1) DPconn1DP1 = FF_DeleteP_ID[2];
                        }
                        if (connDeleteP_Count3 == 1)
                        {
                            if (DPconn1DP0 == -1) DPconn1DP0 = FF_DeleteP_ID[3];
                            else if (DPconn1DP1 == -1) DPconn1DP1 = FF_DeleteP_ID[3];
                        }
                        #endregion

                        List<int> tempFF_DeleteP_ID = new();

                        #region Find the line's first DeletePoint
                        if (DPconn1DP0 < DPconn1DP1) tempFF_DeleteP_ID.Add(DPconn1DP0);
                        else tempFF_DeleteP_ID.Add(DPconn1DP1);
                        #endregion

                        #region Rearrange
                        for (int i = 0; i < 3; i++)
                        {
                            List<int> connP = new();
                            if (tempFF_DeleteP_ID[i] == FF_DeleteP_ID[0]) connP = connP0;
                            if (tempFF_DeleteP_ID[i] == FF_DeleteP_ID[1]) connP = connP1;
                            if (tempFF_DeleteP_ID[i] == FF_DeleteP_ID[2]) connP = connP2;
                            if (tempFF_DeleteP_ID[i] == FF_DeleteP_ID[3]) connP = connP3;

                            for (int j = 0; j < FF_DeleteP_ID.Count; j++)
                            {
                                if (connP.Contains(FF_DeleteP_ID[j]) && !tempFF_DeleteP_ID.Contains(FF_DeleteP_ID[j]))
                                {
                                    tempFF_DeleteP_ID.Add(FF_DeleteP_ID[j]);
                                    break;
                                }
                            }
                        }
                        #endregion

                        FF_DeleteP_ID = tempFF_DeleteP_ID;
                        #endregion

                        #region Find the MidPoint opposite to the middle part of the line
                        int oppEP = -1;

                        for (int i = 0; i < 6; i++)
                        {
                            List<int> FP = new();
                            FP.Add(MC_Data.FacePs[i * 4]);
                            FP.Add(MC_Data.FacePs[i * 4 + 1]);
                            FP.Add(MC_Data.FacePs[i * 4 + 2]);
                            FP.Add(MC_Data.FacePs[i * 4 + 3]);

                            // Find the Face which contains last three DeletePoints in the line
                            if (FP.Contains(FF_DeleteP_ID[1]) && FP.Contains(FF_DeleteP_ID[2]) && FP.Contains(FF_DeleteP_ID[3]))
                            {
                                #region Find the opposite MidPoint
                                List<int> connMidP2 = new();
                                connMidP2.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3]);
                                connMidP2.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3 + 1]);
                                connMidP2.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[2] * 3 + 2]);
                                List<int> connMidP3 = new();
                                connMidP3.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[3] * 3]);
                                connMidP3.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[3] * 3 + 1]);
                                connMidP3.Add(MC_Data.PconnMidPs[FF_DeleteP_ID[3] * 3 + 2]);

                                for (int j = 0; j < 4; j++)
                                {
                                    int FEP = MC_Data.FaceMidPs[i * 4 + j];
                                    if (connMidP3.Contains(FEP) && !connMidP2.Contains(FEP))
                                    {
                                        oppEP = FEP;
                                        break;
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region Build Triangles
                        Triangles.AddRange(LE0);
                        Triangles.Add(oppEP);
                        Triangles.AddRange(LE1);
                        Triangles.Add(oppEP);

                        for (int i = 0; i < FF_MidP.Count; i++)
                        {
                            if (!Triangles.Contains(FF_MidP[i]))
                            {
                                int preP = -1, nexP = -1;
                                if (i > 0) preP = FF_MidP[i - 1];
                                else preP = FF_MidP[FF_MidP.Count - 1];
                                if (i < FF_MidP.Count - 1) nexP = FF_MidP[i + 1];
                                else nexP = FF_MidP[0];

                                Triangles.Add(preP);
                                Triangles.Add(FF_MidP[i]);
                                Triangles.Add(nexP);

                                Triangles.Add(preP);
                                Triangles.Add(nexP);
                                Triangles.Add(oppEP);
                                break;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                Basic_MPT_DealCount += FF_DeleteP_ID.Count;
            }
        }

        if (inverse)
        {
            // Flip FillFace Triangles' normal
            for (int i = 0; i < Triangles.Count - 2; i += 3)
            {
                int T0 = Triangles[i];
                int T1 = Triangles[i + 1];
                int T2 = Triangles[i + 2];

                Triangles[i] = T2;
                Triangles[i + 1] = T1;
                Triangles[i + 2] = T0;
            }
        }

        return Triangles.ToArray();
    }
}
