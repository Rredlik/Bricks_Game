using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{

    public static InputController Instance { get; private set; }

    bool touchMoved = false, touching = false;

    Vector2 lastTouchPosition;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Touching())
        {
            Vector2 touchPosition = GetTouchPosition();

            if (!touching)
            {
                lastTouchPosition = touchPosition;
                touchMoved = false;
                touching = true;
                return;
            }

            if(touchPosition != lastTouchPosition)
            {
                touchMoved = true;
            }

            lastTouchPosition = touchPosition;
        }
        else
        {
            touching = false;
        }
    }

    public bool TouchedSingleStationary()
    {
        if (touchMoved)
        {
            return false;
        }

        if (Input.touchSupported)
        {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
        }
        else
        {
            return Input.GetMouseButtonUp(0);
        }

    }

    bool Touching()
    {
        if (Input.touchSupported)
        {
            return Input.touchCount > 0;
        }
        else
        {
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
        }
    }

    public bool TouchIsOverControlPanel()
    {
        Vector2 pos = GetTouchPosition();
        EventSystem eventSystem = EventSystem.current;
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = pos;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, raycastResults);

        if(raycastResults != null && raycastResults.Count > 0)
        {
            return raycastResults[0].gameObject.name == "ControlPanel";
        }
        else
        {
            return false;
        }
    }

    public static Vector2 GetTouchPosition()
    {
        if (Input.touchSupported)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }
}