using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minView = 20f, maxView = 90f;

    public float rotationSpeed = 5f;

    [Space]
    public Camera _camera;

    bool isRotation = false, isZoom = false;
    Vector2 lastFirstTouchPosition = Vector2.zero;
    float lastZoomDistance;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            StopRotation();
            UpdateZoom();
        }
        else if (Input.touchCount == 1 || Input.GetMouseButton(0))
        {
            StopZoom();
            UpdateRotation();
        }
        else
        {
            StopRotation();
            StopZoom();
        }
    }

    void UpdateRotation()
    {
        Vector2 touchPosition = InputController.GetTouchPosition();

        if (!isRotation)
        {
            isRotation = true;
            lastFirstTouchPosition = touchPosition;
            return;
        }

        Vector2 delta = touchPosition - lastFirstTouchPosition;
        lastFirstTouchPosition = touchPosition;

        Vector2 direction = delta.normalized;
        Vector3 worldDirection = new Vector3(-direction.y, direction.x);
        Vector3 newEulers = transform.localEulerAngles + ((Time.deltaTime * rotationSpeed) * worldDirection);
        if(newEulers.x < 0f)
        {
            newEulers.x = 0f;
        }
        transform.localEulerAngles = newEulers;
    }

    void StopRotation()
    {
        isRotation = false;
    }

    void UpdateZoom()
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPosition = firstTouch.position;
        Vector2 secondTouchPosition = secondTouch.position;

        float zoomDistance = Vector2.Distance(firstTouchPosition, secondTouchPosition);

        if (!isZoom)
        {
            isZoom = true;
            lastZoomDistance = zoomDistance;
            return;
        }

        float zoomDelta = zoomDistance - lastZoomDistance;
        lastZoomDistance = zoomDistance;

        float zoom = _camera.fieldOfView - (zoomDelta * zoomSpeed * Time.deltaTime);
        _camera.fieldOfView = Mathf.Clamp(zoom, minView, maxView);
    }

    void StopZoom()
    {
        if (isZoom)
        {
            isZoom = false;
        }
    }

    private void Reset()
    {
        _camera = GetComponentInChildren<Camera>();
    }
}
