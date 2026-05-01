using UnityEngine;

/// <summary>
/// Единая точка для мыши (Editor/ПК), Device Simulator и первого пальца на Android/iOS.
/// </summary>
public static class PrimaryPointerInput
{
    public static bool GetPrimaryDownThisFrame(out Vector2 screenPosition)
    {
        screenPosition = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            screenPosition = t.position;
            if (t.phase == TouchPhase.Began)
                return true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            screenPosition = Input.mousePosition;
            return true;
        }

        return false;
    }

    public static bool GetPrimaryUpThisFrame(out Vector2 screenPosition)
    {
        screenPosition = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            screenPosition = t.position;
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                return true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            screenPosition = Input.mousePosition;
            return true;
        }

        return false;
    }

    public static Vector2 GetPrimaryScreenPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Input.mousePosition;
    }
}