using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MapEditor : MonoBehaviour {

    public Color[] colors;
    public HexGrid hexGrid;
    private Color curColor;
    private int curElevation;
    void Awake()
    {
        SelectColor(0);
        SetElevation(0);
    }
	
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    void EditCell(HexCell cell)
    {
        cell.Elevation = curElevation;
        cell.color = curColor;
        hexGrid.Refresh();
    }

    public void SetElevation(float elevation)
    {
        curElevation = (int)elevation;
    }

    public void SelectColor(int choice)
    {
        curColor = colors[choice];
    }
}
