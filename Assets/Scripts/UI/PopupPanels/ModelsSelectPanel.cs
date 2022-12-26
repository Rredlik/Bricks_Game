using UnityEngine;

public class ModelsSelectPanel : BaseGameplayPanel
{
    public DefaultPanelButton original;
    public Transform buttonsParent;

    DefaultPanelButton[] buttons;

    protected override void Init()
    {
        buttons = new DefaultPanelButton[ModelsController.Instance.models.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(original, buttonsParent);
            BricksModelDataReference modelData = ModelsController.Instance.models[i];

            buttons[i].buttonImage.sprite = modelData.icon;
            int index = i;
            buttons[i].button.onClick.AddListener(() => Select(index));
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
        buttons[index].button.interactable = !(index == ModelsController.Instance.SelectedModelIndex);
    }

    void Select(int index)
    {
        ModelsController.Instance.StartBuildingModel(index);
        SetData();
    }
}
