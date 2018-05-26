using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {
    
	public int width = 5;
	public int height = 5;

	public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor;

    HexMesh hexMesh;
    HexCell[] cells;
    Canvas gridCanvas;
    
    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[height * width];
        int i = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;

        position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f);//every second row, first cell shift 1 iR = 1/2 *(2iR) to the right 
        //- x-Axis still orthogonal to z-Axis(=> HexCoordinates: FromOffsetCoordinates)
        
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);

        cell.coord = HexCoordinates.FromOffsetCoordinates(x, z); //get right Coordinate in right coordinate system x/y/z
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.color = defaultColor;

        //Label:
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coord.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
        
        //Neighbor Relation:
        if(x > 0) //W-E - connections
        {
            cell.SetNeighbor(HexDirection.W, cells[i-1]);
        }
        if(z > 0)
        {
            if((z & 1) == 0)//even rows 
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);//=> NW to SE
                if (x > 0)//NE to SW
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else //uneven rows
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);// => NE to SW
                if (x < width-1)//SW to NE
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }


    }


    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);//Transform from worldspace to localspace
        HexCoordinates hexPosition = HexCoordinates.FromPosition(position);
        int index = hexPosition.Z * width + (hexPosition.X + hexPosition.Z / 2);//hexPos Z * w => Z rows of cells | x + z/2 because of X-axis shift
        return cells[index];
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }
    //obsolete
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);//Transform from worldspace to localspace
        HexCoordinates hexPosition = HexCoordinates.FromPosition(position);
        int index = hexPosition.Z * width + (hexPosition.X + hexPosition.Z / 2);//hexPos Z * w => Z rows of cells | x + z/2 because of X-axis shift
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);//little bit inefficient 
    }

}
