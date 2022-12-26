using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public List<Brick> bricks;

    [HideInInspector] public GridVector gridPosition;
    [HideInInspector] public BoxCollider boxCollider;

    [HideInInspector] public GridCellsContainer container;

    void Awake()
    {
        bricks = new List<Brick>();
    }

    public bool IsFree(int orderIndex)
    {
        return bricks == null || bricks.Count <= orderIndex || bricks[orderIndex] == null;
    }

    public bool ContainsPoint(Vector3 point)
    {
        Vector3 cellHalfSize = boxCollider.size / 2f;
        Vector3 corner1 = transform.position - cellHalfSize;
        Vector3 corner2 = transform.position + cellHalfSize;

        if(corner1.x > point.x || corner2.x < point.x)
        {
            return false;
        }

        if (corner1.z > point.z || corner2.z < point.z)
        {
            return false;
        }

        return true;
    }

    public Brick AddBrickOnOrder(int orderIndex, int brickIndex = -1, BrickClass brickclass = BrickClass.Default)
    {
        Brick brick = GameController.Instance.CreateBrick(brickIndex);
        brick.ownerCell = this;
        brick.orderIndex = orderIndex;
        brick.brickClass = brickclass;
        brick.name = "Brick " + gridPosition.x + " " + gridPosition.z + " " + orderIndex + " " + brickIndex;

        Vector3 cellSize = boxCollider.size;
        Vector3 cellCornerPosition = transform.position;
        cellCornerPosition.z -= cellSize.z / 2f;
        cellCornerPosition.x -= cellSize.x / 2f;

        Vector3 brickPointOffset = brick.positionPoint.position - brick.transform.position;
        Vector3 position = cellCornerPosition - brickPointOffset;

        Vector3 brickSize = brick.meshRenderer.bounds.size;
        float height = brickSize.y;
        position.y = height * orderIndex;
        brick.transform.position = position;

        AddBrickOnOrder(orderIndex, brick);

        return brick;
    }

    public void AddBrickOnOrder(int orderIndex, Brick brick)
    {
        if (bricks.Count > orderIndex)
        {
            if (bricks[orderIndex] == null)
            {
                bricks[orderIndex] = brick;
            }
            else
            {
                print("brick is already exists");
            }

        }
        else
        {
            for (int i = bricks.Count; i < orderIndex; i++)
            {
                bricks.Add(null);
            }

            bricks.Add(brick);
        }

        brick.cells.Add(this);
    }

    public void RemoveBrick(Brick brick, bool destroy = true)
    {
        //if(bricks.Count <= brick.orderIndex)
        //{
        //    return;
        //}

        bricks[brick.orderIndex] = null;
        if (destroy)
        {
            Destroy(brick.gameObject);
        }

        while(bricks.Count > 0)
        {
            if(bricks[bricks.Count - 1] == null)
            {
                bricks.RemoveAt(bricks.Count - 1);
            }
            else
            {
                break;
            }
        }
    }
}
