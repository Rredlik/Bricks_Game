using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BrickData
{
    public GridVector position;
    public int prefabIndex;
    public float rotationAngle;
}


[CreateAssetMenu(fileName = "New BricksModel", menuName = "Game Data/Bricks Model")]
public class BricksModelData : ScriptableObject
{
    public BrickData[] bricks;
}