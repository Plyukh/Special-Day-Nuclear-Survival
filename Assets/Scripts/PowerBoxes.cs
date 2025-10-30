using UnityEngine;
using System.Collections;

public class PowerBoxes : Interactable
{
    public bool works;
    [SerializeField] private GameObject effect;

    [SerializeField] private Light[] lights;
    private float[] startRanges;
    private float[] addValues;

    new public void OnEnable()
    {
        base.OnEnable();
    }

    public void StartPowerBoxes()
    {
        startRanges = new float[lights.Length];
        addValues = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            startRanges[i] = lights[i].range;
            addValues[i] = lights[i].range / 100 * 10;
        }
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].range = 0;
        }
        Use();
    }

    public override void Use()
    {
        if (works)
        {
            ActiveLights(false);
            effect.SetActive(true);
            works = false;
        }
        else
        {
            ActiveLights(true);
            effect.SetActive(false);
            works = true;
        }
    }

    public void ActiveLights(bool value)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            addValues[i] = startRanges[i] / 100 * 5;
            lights[i].transform.parent.parent.GetComponent<Room>().light = value;
        }
        StartCoroutine(LightsCoroutine(value));
    }

    IEnumerator LightsCoroutine(bool value)
    {
        if (value == false)
        {
            for (int i = 0; i < addValues.Length; i++)
            {
                addValues[i] *= -1;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.025f);
            for (int j = 0; j < lights.Length; j++)
            {
                lights[j].range += addValues[j];
            }
        }

        StopCoroutine(LightsCoroutine(value));
    }
}