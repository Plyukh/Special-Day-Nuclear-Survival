using UnityEngine;
using System.Collections;

public class CubeObject : Interactable
{
    public Door door;
    private bool click;

    private Renderer targetRenderer;
    private Room cachedRoom;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        cachedRoom = GetComponentInParent<Room>();
    }

    public void ClickMesh()
    {
        outline.OutlineWidth = 3;
        click = true;
    }

    public new void Update()
    {
        base.Update();

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

        if (cachedRoom == null)
            cachedRoom = GetComponentInParent<Room>();

        if (cachedRoom != null && cachedRoom.find && fadeRoutine == null && targetRenderer != null && gameObject.activeInHierarchy)
            fadeRoutine = StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        const float alphaStep = 2f / 255f;

        while (targetRenderer != null && targetRenderer.material.color.a > alphaStep)
        {
            var c = targetRenderer.material.color;
            c.a = Mathf.Max(0f, c.a - alphaStep);
            targetRenderer.material.color = c;
            yield return null;
        }

        if (targetRenderer != null)
        {
            var c = targetRenderer.material.color;
            c.a = 0f;
            targetRenderer.material.color = c;
        }

        gameObject.SetActive(false);
        fadeRoutine = null;
    }
}
