using UnityEngine;
using UnityEngine.UI;

public class Radiation : MonoBehaviour
{
    [SerializeField] private Animator radiationIcon;
    [SerializeField] private Slider radiationSlider;

    private float maxRad = 1000;
    public float currentRad;
    public float rad;

    public void StartRad()
    {
        radiationSlider.maxValue = maxRad;

        if (currentRad > 0)
        {
            radiationIcon.SetBool("Rad", true);
            if (GetComponent<CharacterMovement>().CurrentRoom.GetComponent<Room>().radiactive)
            {
                StartRadiation();
            }
            else
            {
                StopRadiation();
            }
        }
    }

    private void Update()
    {
        float newRad = 0;

        if (rad > 0)
        {
            EffectsUI.RadiationEffect(true);

            if (GetComponent<Character>().currentArmor != null)
            {
                newRad = rad / 100 * GetComponent<Character>().currentArmor.protection[4] + GetComponent<HealthSystem>().protection[4];
            }
            else if(GetComponent<HealthSystem>().protection[4] > 0)
            {
                newRad = rad / 100 * GetComponent<HealthSystem>().protection[4];
            }
        }
        else
        {
            EffectsUI.RadiationEffect(false);
        }

        currentRad += (rad - newRad) * Time.deltaTime;
        radiationSlider.value = currentRad;

        if (currentRad >= maxRad)
        {
            currentRad = maxRad - 1;
            rad = 0;
            radiationIcon.SetBool("Rad", false);
            GetComponent<HealthSystem>().Death();
        }
        if (currentRad <= 0)
        {
            currentRad = 0;
            radiationIcon.SetBool("Rad",false);
        }
    }

    public void StartRadiation()
    {
        rad = 20;
        radiationIcon.SetBool("Rad", true);
    }
    public void StopRadiation()
    {
        rad = -10;
    }
}