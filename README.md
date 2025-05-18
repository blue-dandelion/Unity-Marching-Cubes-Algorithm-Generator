# Marching Cubes Algorithm Generator in Unity

![image](https://github.com/user-attachments/assets/14483ac0-91a2-4b0f-b3c1-2a44eb7f72f7)

This open-source Unity project demonstrates the Marching Cubes algorithm in a simple and interactive way. It allows users to select individual cube vertices and observe how the cube's mesh changes when a point is removed. This tool provides a hands-on visualisation to help users understand how the Marching Cubes algorithm works and how different configurations affect the generated surface. It also demonstrates how the Marching Cubes algorithm is implemented, highlighting the relationships between vertices, edges, and faces.

Ideal for students, developers, and anyone interested in 3D graphics or procedural mesh generation.

## Features:
![image](https://github.com/user-attachments/assets/6b0bc416-4f85-47c2-95d4-869569485080)

1. Drag <ins>MarchingCube.cs</ins> from Assets to an empty GameObject. It will automatically generate 8 points in the Scene.
2. Click on the GameObject to let the Inspector show the GameObject's properties, and lock the inspector.
3. Click <ins>Build a basic Marching Cube</ins> button in the inspector. It will generate meshes to create a cube between the 8 points
4. Select the marching cube points that you want to remove in the Unity Editor
5. Click <ins>Delete selected Points</ins> button in the inspector, the cube will transform based on the Marching Cubes Algorithm
