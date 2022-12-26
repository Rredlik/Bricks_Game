using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsContainer : MonoBehaviour
{
    public GridVector size;

    public SpriteRenderer spriteRenderer;

    [HideInInspector] public GridCell[] cells;

    Transform cellsParent;

    public void CreateCells(int startZ, int startX)
    {
        Vector3 size = spriteRenderer.bounds.size;
        float cellSize = size.x / 2f;
        float offset = cellSize / 2f;

        cellsParent = new GameObject("Cells").transform;
        cellsParent.SetParent(transform);
        cellsParent.localPosition = Vector3.zero;
        cellsParent.localEulerAngles = transform.localEulerAngles * -1f;

       cells = new GridCell[4];

        cells[0] = CreateCell(cellSize, -offset, -offset, startZ, startX);
        cells[1] = CreateCell(cellSize, -offset, offset, startZ, startX + 1);
        cells[2] = CreateCell(cellSize, offset, -offset, startZ + 1, startX);
        cells[3] = CreateCell(cellSize, offset, offset, startZ + 1, startX + 1);
    }

    GridCell CreateCell(float size, float zPosition, float xPosition, int zIndex, int xIndex)
    {
        GameObject obj = new GameObject("Cell " + zIndex + " " +xIndex);
        obj.transform.SetParent(cellsParent);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.SetAsLastSibling();
        obj.transform.localPosition = new Vector3(xPosition, 0f, zPosition);

        GridCell cell = obj.AddComponent<GridCell>();
        cell.gridPosition = new GridVector(xIndex, zIndex);
        cell.container = this;
        cell.boxCollider = obj.AddComponent<BoxCollider>();
        cell.boxCollider.size = new Vector3(size, size, size);

        return cell;
    }
}
