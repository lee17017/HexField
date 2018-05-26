using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public RectTransform uiRect;
    public HexCoordinates coord;
    public Color color;

    private int elevation;

    [SerializeField]
    HexCell[] neighbors;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            transform.position = new Vector3(transform.localPosition.x, elevation * HexMetrics.elevationStep, transform.localPosition.z);

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(this.elevation, neighbors[(int)direction].Elevation);
    }
}
