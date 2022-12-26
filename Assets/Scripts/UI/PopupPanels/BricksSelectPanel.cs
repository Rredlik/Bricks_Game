using UnityEngine;

public class BricksSelectPanel : BaseGameplayPanel
{
    public DefaultPanelButton original;
    public Transform buttonsParent;

    DefaultPanelButton[] buttons;

    protected override void Init()
    {
        buttons = new DefaultPanelButton[DataController.Instance.bricks.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(original, buttonsParent);
            BrickItem brickData = DataController.Instance.GetBrickData(i);
            //buttons[i].buttonText.text = brickData.brickPrefab.size.z + "x" + brickData.brickPrefab.size.x;
            string picPath;

            if (i == 9 || i == 10)
            {
                picPath = "cubenaklon" + brickData.brickPrefab.size.z + "x" + brickData.brickPrefab.size.x;
            }
            else
            {
                picPath = "cube" + brickData.brickPrefab.size.z + "x" + brickData.brickPrefab.size.x;
            }
            var sprite = Resources.Load<Sprite>(picPath);
            buttons[i].buttonImage.sprite = sprite;
            SetButton(i);
        }

        Destroy(original.gameObject);
    }

    protected override void SetData()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            SetButton(i);
        }
    }

    void SetButton(int index)
    {
        BrickItem brickData = DataController.Instance.GetBrickData(index);
        DefaultPanelButton brickButton = buttons[index];

        brickButton.button.onClick.RemoveAllListeners();
        if (index == DataController.Instance.GetSelectedBrickIndex())
        {
            brickButton.button.interactable = false;
        }
        else
        {
            brickButton.button.onClick.AddListener(() => Select(index));
            brickButton.button.interactable = true;
        }
    }

    void Select(int index)
    {
        DataController.Instance.SelectBrick(index);
        SetData();
    }
}
