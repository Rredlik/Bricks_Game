using System.Collections;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public MainPanel mainPanel;

    public static UIController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
}