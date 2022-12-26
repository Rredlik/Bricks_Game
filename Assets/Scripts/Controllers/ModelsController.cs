using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BricksModelDataReference
{
    public Sprite icon;
    public string prefabName;
}

public class ModelsController : MonoBehaviour
{
    public static ModelsController Instance { get; private set; }

    public int SelectedModelIndex { get; private set; }

    public BricksModelDataReference[] models;

    int  currentYLevel = 0, startAreaYLevel, currentProgress = 0;
    GridVector startAreaPosition, startModelPosition;
    BricksModelData modelData;

    List<BrickData> currentYBricks;

    const string modelsPath = "BricksModels/";

    private void Awake()
    {
        Instance = this;

        SelectedModelIndex = -1;
    }

    private void Start()
    {
        GameController.Instance.OnBrickTouched += OnBrickTouched;
    }

    public void StartBuildingModel(int index)
    {
        if (modelData != null)
        {
            CompleteModel();
        }

        SelectedModelIndex = index;
        StartBuildingSelectedModel();
    }

    void StartBuildingSelectedModel()
    {
        string modelName = models[SelectedModelIndex].prefabName;
        modelData = Resources.Load<BricksModelData>(modelsPath + modelName);
        if(modelData == null)
        {
            print("Model " + modelName + " not found!");
            return;
        }

        CalculatePositionsOnArea();

        currentYLevel = GetMinYPosition();
        CreateCurrentYLevelBricks();

        currentProgress = 0;
        UIController.Instance.mainPanel.SetModelPanelState(true);
        SetProgressText();
    }

    void CalculatePositionsOnArea()
    {
        BrickData[] bricks = modelData.bricks;

        int minX = 0;
        int maxX = 0;
        int minZ = 0;
        int maxZ = 0;

        for (int i = 0; i < bricks.Length; i++)
        {
            BrickData brickData = bricks[i];
            GridVector pos = brickData.position;

            GridVector brickSize = GameController.Instance.GetPrefabSizeWithRotation(brickData.prefabIndex, brickData.rotationAngle);
            int maxBrickX = pos.x + brickSize.x;
            int maxBrickZ = pos.z + brickSize.z;

            if (i <= 0)
            {
                minX = pos.x;
                maxX = maxBrickX;
                minZ = pos.z;
                maxZ = maxBrickZ;

                continue;
            }

            if (pos.x < minX)
            {
                minX = pos.x;
            }
            if (maxBrickX > maxX)
            {
                maxX = maxBrickX;
            }
            if (pos.z < minZ)
            {
                minZ = pos.z;
            }
            if (maxBrickZ > maxZ)
            {
                maxZ = maxBrickZ;
            }
        }

        GridVector minBound = new GridVector(minX, minZ);
        GridVector maxBound = new GridVector(maxX, maxZ);

        startModelPosition = minBound;

        int sizeX = maxBound.x - minBound.x;
        int sizeZ = maxBound.z - minBound.z;

        int center = GameController.Instance.gridSize;

        startAreaPosition = new GridVector(center - (sizeX / 2), center - (sizeZ / 2));

        int orderIndex = 0;
        startAreaYLevel = 0;
        while(orderIndex <= 500)
        {
            if(YLevelOnAreaIsFree(minBound, maxBound, orderIndex))
            {
                startAreaYLevel = orderIndex;
                break;
            }

            orderIndex++;
        }
    }

    bool YLevelOnAreaIsFree(GridVector minBound, GridVector maxBound, int orderIndex)
    {
        for (int z = minBound.z; z <= maxBound.z; z++)
        {
            for (int x = minBound.x; x <= maxBound.x; x++)
            {
                GridVector cellPos = ModelPositionToAreaPosition(new GridVector(x, z));
                if(!GameController.Instance.IsFreeCell(cellPos, orderIndex))
                {
                    return false;
                }
            }
        }

        return true;
    }

    int GetMinYPosition()
    {
        BrickData[] bricks = modelData.bricks;
        int minY = bricks[0].position.y;
        for (int i = 1; i < bricks.Length; i++)
        {
           int y = bricks[i].position.y;
            if(y < minY)
            {
                minY = y;
            }
        }

        return minY;
    }

    void FindCurrentYBricks()
    {
        BrickData[] bricks = modelData.bricks;

        currentYBricks = new List<BrickData>();

        int orderIndex = GetCurrentYLevelIndex();

        for (int i = 0; i < bricks.Length; i++)
        {
            BrickData brickData = bricks[i];
            GridVector pos = brickData.position;

            if (pos.y != orderIndex)
            {
                continue;
            }

            currentYBricks.Add(brickData);
        }
    }

    int GetCurrentYLevelIndex()
    {
        return startAreaYLevel + currentYLevel;
    }

    void CreateCurrentYLevelBricks()
    {
        int orderIndex = GetCurrentYLevelIndex();

        FindCurrentYBricks();
        for (int i = 0; i < currentYBricks.Count; i++)
        {
            BrickData brickData = currentYBricks[i];
            GridVector pos = ModelPositionToAreaPosition(brickData.position);
            Brick newBrick = GameController.Instance.CreateBrickInCell(pos, orderIndex, brickData.prefabIndex, BrickClass.ModelPart);
            newBrick.SetAsSilhouette();
        }
    }

    void SetProgressText()
    {
        UIController.Instance.mainPanel.modelProgressText.text = currentProgress + "/" + modelData.bricks.Length;
    }

    void CheckCurrentYLevelProgress()
    {
        int orderIndex = GetCurrentYLevelIndex();

        for (int i = 0; i < currentYBricks.Count; i++)
        {
            BrickData brickData = currentYBricks[i];
            List<Brick> createdBricks = GameController.Instance.GetCell(ModelPositionToAreaPosition(brickData.position)).bricks;
            if (createdBricks == null)
            {
                return;
            }

            if (createdBricks[orderIndex].Silhouette)
            {
                return;
            }
        }

        ToNextYLevel();
    }

    void OnBrickTouched(Brick brick)
    {
        if(brick.brickClass != BrickClass.ModelPart || !brick.Silhouette)
        {
            return;
        }

        brick.SetDefault();
        currentProgress++;
        SetProgressText();

        if (currentProgress >= modelData.bricks.Length)
        {
            CompleteModel();
            return;
        }

        CheckCurrentYLevelProgress();
    }

    void ToNextYLevel()
    {
        int lastYLevel = currentYLevel;
        int newYLevel = -1;
        BrickData[] bricks = modelData.bricks;
        for (int i = 0; i < bricks.Length; i++)
        {
            BrickData brickData = bricks[i];
            GridVector pos = brickData.position;
            if(pos.y <= lastYLevel)
            {
                continue;
            }

            if(newYLevel <= lastYLevel)
            {
                newYLevel = pos.y;
                continue;
            }

            if(pos.y < newYLevel)
            {
                newYLevel = pos.y;
            }
        }

        currentYLevel = newYLevel;
        CreateCurrentYLevelBricks();
    }

    void CompleteModel()
    {
        SetDefaultClassToBricks();

        UIController.Instance.mainPanel.SetModelPanelState(false);

        SelectedModelIndex = -1;
        currentYLevel = 0;
        startAreaYLevel = 0;
        currentProgress = 0;
        modelData = null;
        currentYBricks = null;
    }

    void SetDefaultClassToBricks()
    {
        BrickData[] bricks = modelData.bricks;

        for (int i = 0; i < bricks.Length; i++)
        {
            BrickData brickData = bricks[i];
            List<Brick> createdBricks = GameController.Instance.GetCell(ModelPositionToAreaPosition(brickData.position)).bricks;
            if (createdBricks == null)
            {
                continue;
            }
            for (int j = 0; j < createdBricks.Count; j++)
            {
                Brick _brick = createdBricks[j];
                if (_brick != null && _brick.brickClass == BrickClass.ModelPart)
                {
                    if (_brick.Silhouette)
                    {
                        _brick.Remove();
                    }
                    _brick.brickClass = BrickClass.Default;
                }
            }
        }
    }

    GridVector ModelPositionToAreaPosition(GridVector pos)
    {
        return startAreaPosition.Add(pos.Subtract(startModelPosition));
    }

    private void OnDestroy()
    {
        GameController.Instance.OnBrickTouched -= OnBrickTouched;
    }
}
