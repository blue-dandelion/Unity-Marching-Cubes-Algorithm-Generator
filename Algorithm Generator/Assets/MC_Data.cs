using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct MC_Data
{
    // A basic Marching Cube's Triangles
    public static int[] BasicTriangles =
    {
        0, 1, 3,    // Triangle 0
        1, 2, 3,    // Triangle 1
        0, 4, 5,    // Triangle 2
        1, 0, 5,    // Triangle 3
        1, 6, 2,    // Triangle 4
        1, 5, 6,    // Triangle 5
        2, 7, 3,    // Triangle 6
        2, 6, 7,    // Triangle 7
        0, 3, 4,    // Triangle 8
        3, 7, 4,    // Triangle 9
        4, 7, 6,    // Triangle 10
        4, 6, 5     // Triangle 11
    };

    // Every Point's connected Points
    public static int[] PconnPs =
    {
        1, 3, 4,    // Point 0
        0, 2, 5,    // Point 1
        1, 3, 6,    // Point 2
        0, 2, 7,    // Point 3
        0, 5, 7,    // Point 4
        1, 4, 6,    // Point 5
        2, 5, 7,    // Point 6
        3, 4, 6     // Point 7
    };

    // Every Point's connected EdgePoints
    public static int[] PconnMidPs =
    {
        8,  11, 12, // Point 0
        8,  13, 9,  // Point 1
        9,  14, 10, // Point 2
        10, 15, 11, // Point 3
        12, 19, 16, // Point 4
        13, 16, 17, // Point 5
        14, 17, 18, // Point 6
        15, 18, 19  // Point 7
    };

    // Every Face's contained Points
    public static int[] FacePs =
    {
        0, 1, 2, 3, // Face 0
        1, 0, 4, 5, // Face 1
        1, 5, 6, 2, // Face 2
        3, 2, 6, 7, // Face 3
        0, 3, 7, 4, // Face 4
        5, 4, 7, 6  // Face 5
    };

    // Every Face's contained EdgePoints
    public static int[] FaceMidPs =
    {
        8,  9, 10, 11,  // Face 0
        8,  12, 16, 13, // Face 1
        9,  13, 17, 14, // Face 2
        10, 14, 18, 15, // Face 3
        11, 15, 19, 12, // Face 4
        16, 19, 18, 17  // Face 5
    };

    // Every Face's contained Triangles
    public static int[] FaceTriangles =
    {
        0, 1, 3,    1, 2, 3,    // Face 0
        0, 4, 5,    1, 0, 5,    // Face 1
        1, 6, 2,    1, 5, 6,    // Face 2
        2, 7, 3,    2, 6, 7,    // Face 3
        0, 3, 4,    3, 7, 4,    // Face 4
        4, 7, 6,    4, 6, 5     // Face 5
    };
}
