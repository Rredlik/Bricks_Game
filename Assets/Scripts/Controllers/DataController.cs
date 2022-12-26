using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GameAction();

[System.Serializable]
public struct BrickItem
{
    public Brick brickPrefab;
}

public class DataController : MonoBehaviour
{
    public BrickItem[] bricks;

    int currentBrickIndex = -1;

    public event GameAction OnBrickSelected;

    public static DataController Instance { get; private set; }

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentBrickIndex = PlayerPrefs.GetInt("CurrentBrick", 0);
    }

    public void SelectBrick(int index)
    {
        currentBrickIndex = index;
        PlayerPrefs.SetInt("CurrentSkin", index);
        OnBrickSelected?.Invoke();
    }

    public BrickItem GetSelectedBrick()
    {
        return bricks[currentBrickIndex];
    }

    public Brick GetSelectedBrickPrefab()
    {
        return GetSelectedBrick().brickPrefab;
    }

    public BrickItem GetBrickData(int index)
    {
        return bricks[index];
    }

    public int GetSelectedBrickIndex()
    {
        return currentBrickIndex;
    }
}
