using UnityEngine;

public class EffectsUI : MonoBehaviour
{
    static GameObject encumberedEffect;
    static GameObject notEnoughPowerEffect;
    static GameObject radiationEffect;
    static GameObject goodSteathEffect;
    static GameObject badSteathEffect;
    static GameObject accuracySteathEffect;

    private void Start()
    {
        encumberedEffect = transform.GetChild(0).gameObject;
        notEnoughPowerEffect = transform.GetChild(1).gameObject;
        radiationEffect = transform.GetChild(2).gameObject;
        goodSteathEffect = transform.GetChild(3).gameObject;
        badSteathEffect = transform.GetChild(4).gameObject;
        accuracySteathEffect = transform.GetChild(5).gameObject;
    }

    public static void EncumberedEffect(bool value)
    {
        encumberedEffect.SetActive(value);
    }
    public static void NotEnoughPowerEffect(bool value)
    {
        notEnoughPowerEffect.SetActive(value);
    }
    public static void RadiationEffect(bool value)
    {
        radiationEffect.SetActive(value);
    }
    public static void SteathEffect(bool value, bool good = true)
    {
        if (value)
        {
            if (good)
            {
                goodSteathEffect.SetActive(true);
                badSteathEffect.SetActive(false);
            }
            else
            {
                goodSteathEffect.SetActive(false);
                badSteathEffect.SetActive(true);
            }
        }
        else
        {
            goodSteathEffect.SetActive(false);
            badSteathEffect.SetActive(false);
        }
    }
    public static void AccuracySteathEffect(bool value)
    {
        accuracySteathEffect.SetActive(value);
    }
}