using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CubeObject : Interactable
{
    public Door door;
    private bool click;

    public void ClickMesh()
    {
        outline.OutlineWidth = 3;
        click = true;
    }

    public new void Update()
    {
        if (click)
        {
            if (outline.OutlineWidth >= 10)
            {
                outline.OutlineWidth = 3;
                click = false;
            }
            else
            {
                outline.OutlineWidth += 0.5f;
            }
        }
    }
}