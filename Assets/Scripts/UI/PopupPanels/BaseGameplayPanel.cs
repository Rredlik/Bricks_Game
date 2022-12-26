using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class BaseGameplayPanel : MonoBehaviour
{
    bool inited = false;

    protected virtual void Init()
    {
    }

    void OnEnable()
    {
        if (inited)
        {
            SetData();
        }
        else
        {
            Init();
            inited = true;
        }

        GameController.Instance.AllowCast(false);
    }

    protected virtual void SetData()
    {
    }

    void OnDisable()
    {
        GameController.Instance.AllowCast(true);
    }
}
