using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;
    MeshCollider meshCollider;

	// Use this for initialization
	void Awake () {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "HexMesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }
	
	public void Triangulate(HexCell[] cellArr) //Triangulate whole mesh
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cellArr.Length; i++)
        {
            Triangulate(cellArr[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    private void Triangulate(HexCell cell) // Triangulate one hex-cell-mesh
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(cell, d);
        }
    }

    private void Triangulate(HexCell cell, HexDirection direction)//one triangle
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(center, v1, v2); // inner big Triangle
        AddTriangleColor(cell.color);

        if (direction <= HexDirection.SE)//NE, E, SE
        {
            TriangulateConnection(cell, direction, v1, v2);
        }
        
        //AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3); //left smaller Triangle
        //AddTriangle(v2, v4, center + HexMetrics.GetSecondCorner(direction)); //right smaller Triangle

        //Coloring:

        //HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        //HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

        //Color edgeColor1 = (cell.color + neighbor.color + prevNeighbor.color) /3f;
        //Color edgeColor2 = (cell.color + neighbor.color + nextNeighbor.color) / 3f;
        
      //  AddTriangleColor(cell.color, edgeColor1, bridgeColor);
       // AddTriangleColor(cell.color, bridgeColor, edgeColor2);
    }

    private void TriangulateConnection(HexCell cell, HexDirection direction, Vector3 v1, Vector3 v2)//Internal Bridges between hex cells and triangles
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if(neighbor == null)
        {
            return;
        }
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = HexMetrics.elevationStep * neighbor.Elevation;

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(v1, v2, v3, v4, cell, neighbor);
        }
        else
        {
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);
        }
        //Triangle to the right of bridge:
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null) //NE, E
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = HexMetrics.elevationStep * nextNeighbor.Elevation;
            AddTriangle(v2, v4, v5);
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
    }

    void TriangulateEdgeTerraces(Vector3 beginLeft, Vector3 beginRight, Vector3 endLeft, Vector3 endRight, HexCell beginCell, HexCell endCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        //First Step
        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(beginCell.color, c2);

        //Steps inbetween
        for (int step = 2; step < HexMetrics.terraceSteps; step++)
        {
            
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;

            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, step);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, step);
            c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, step);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        //Last Step
        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.color);
    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) // add triangle vertices to vert-array and their indices to triangles array
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleColor(Color color)
    {
        //adds 3 colors => on per Triangle vertex(All same color)
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    private void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);

        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    private void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

}
