using UnityEngine;

public class Flashing : MonoBehaviour
{
    public float minIntensity;
    public float maxIntensity;
    public float speed;

    private bool on;
    private Light light;

    private void Start()
    {
        light = GetComponent<Light>();
    }
    void Update()
    {
        if (on == false)
        {
            light.intensity += speed * Time.deltaTime;
            if(light.intensity >= maxIntensity)
            {
                light.intensity = maxIntensity;
                on = true;
            }
        }
        else
        {
            light.intensity -= speed * Time.deltaTime;
            if (light.intensity <= minIntensity)
            {
                light.intensity = minIntensity;
                on = false;
            }
        }
    }
}
