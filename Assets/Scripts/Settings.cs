using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public bool shadows;
    public Text shadowsText;
    public Button shadowsButton;

    public int qualityLevel;
    public Text qualityText;
    public Button qualityButton;

    public int fpsTarget;
    public Text fpsTargetText;
    public Button fpsTargetButton;

    public bool fps;
    public Text fpsText;
    public Button fpsButton;

    [SerializeField] private Text FPSText;

    private int lastFrame;
    private float[] frameDeltaTimeArray;

    public AudioSource music;
    public float musicValue;
    public Text musicText;
    public Slider musicSlider;

    public float soundsValue;
    public Text soundsText;
    public Slider soundsSlider;

    public LanguageManager languageManager;
    public int languageIndex;
    public Text languageText;
    public Button languageButton;
    public SaveScript saveScript;


    public void UpdateSettings()
    {
        if(saveScript.GetComponent<SceneManagerScript>().sceneName != "CharecterCreator")
        {
            frameDeltaTimeArray = new float[50];

            musicSlider.value = musicValue;
            soundsSlider.value = soundsValue;

            Quality(true);
            Shadows();
            FPSTarget();
            FPS();
            Music();
            Sounds();
            SelectLanguage(true);
        }
    }

    private void Update()
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var audioSource in audioSources)
        {
            if (audioSource.name != music.name)
            {
                audioSource.volume = soundsValue / 10;
            }
        }


        if (fps)
        {
            frameDeltaTimeArray[lastFrame] = Time.deltaTime;
            lastFrame = (lastFrame + 1) % frameDeltaTimeArray.Length;
            FPSText.text = "FPS: " + Mathf.RoundToInt(CalculateFPS()).ToString();
        }
    }

    void AddSound()
    {
        transform.parent.GetComponent<AudioSource>().Play();
    }

    private float CalculateFPS()
    {
        float total = 0f;
        foreach (float deltaTime in frameDeltaTimeArray)
        {
            total += deltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }

    public void Quality(bool first = false)
    {
        if (first == false)
        {
            qualityLevel += 1;
        }

        if(qualityLevel > 5)
        {
            qualityLevel = 0;
        }
        QualitySettings.SetQualityLevel(qualityLevel);

        if (qualityLevel == 0)
        {
            qualityText.color = Color.red;
        }
        else if (qualityLevel == 1)
        {
            qualityText.color = Color.red;
        }
        else if (qualityLevel == 2)
        {
            qualityText.color = new Color32(255, 165, 0,255);
        }
        else if (qualityLevel == 3)
        {
            qualityText.color = new Color32(255, 165, 0, 255);
        }
        else if (qualityLevel == 4)
        {
            qualityText.color = Color.green;
        }
        else if (qualityLevel == 5)
        {
            qualityText.color = Color.green;
        }

        UpdateTexts(languageManager.currentLanguage);

        qualityButton.onClick.RemoveAllListeners();
        qualityButton.onClick.AddListener(() => AddSound());
        qualityButton.onClick.AddListener(() => Quality());

        saveScript.SaveSettings();
    }

    public void ShadowsOn()
    {
        shadows = true;
        shadowsText.color = Color.green;
        UpdateTexts(languageManager.currentLanguage);

        Light[] lights = FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var light in lights)
        {
            light.shadows = LightShadows.Hard;
        }

        shadowsButton.onClick.RemoveAllListeners();
        shadowsButton.onClick.AddListener(() => AddSound());
        shadowsButton.onClick.AddListener(() => ShadowsOff());

        saveScript.SaveSettings();
    }
    public void ShadowsOff()
    {
        shadows = false;
        shadowsText.color = Color.red;
        UpdateTexts(languageManager.currentLanguage);

        Light[] lights = FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var light in lights)
        {
            light.shadows = LightShadows.None;
        }

        shadowsButton.onClick.RemoveAllListeners();
        shadowsButton.onClick.AddListener(() => AddSound());
        shadowsButton.onClick.AddListener(() => ShadowsOn());

        saveScript.SaveSettings();
    }

    public void Shadows()
    {
        if (shadows)
        {
            ShadowsOn();
        }
        else
        {
            ShadowsOff();
        }
    }

    public void FPS30()
    {
        fpsTarget = 30;
        fpsTargetText.color = Color.red;
        fpsTargetText.text = "30";

        Application.targetFrameRate = 30;

        fpsTargetButton.onClick.RemoveAllListeners();
        fpsTargetButton.onClick.AddListener(() => AddSound());
        fpsTargetButton.onClick.AddListener(() => FPS60());

        saveScript.SaveSettings();
    }
    public void FPS60()
    {
        fpsTarget = 60;
        fpsTargetText.color = Color.green;
        fpsTargetText.text = "60";

        Application.targetFrameRate = 60;

        fpsTargetButton.onClick.RemoveAllListeners();
        fpsTargetButton.onClick.AddListener(() => AddSound());
        fpsTargetButton.onClick.AddListener(() => FPS30());

        saveScript.SaveSettings();
    }

    public void FPSTarget()
    {
        if(fpsTarget == 30)
        {
            FPS30();
        }
        else
        {
            FPS60();
        }
    }

    public void FPSOn()
    {
        fpsText.color = Color.green;
        UpdateTexts(languageManager.currentLanguage);

        FPSText.gameObject.SetActive(true);

        fpsButton.onClick.RemoveAllListeners();
        fpsButton.onClick.AddListener(() => AddSound());
        fpsButton.onClick.AddListener(() => FPSOff());

        fps = true;

        saveScript.SaveSettings();
    }
    public void FPSOff()
    {
        fps = false;
        fpsText.color = Color.red;
        UpdateTexts(languageManager.currentLanguage);

        FPSText.gameObject.SetActive(false);

        fpsButton.onClick.RemoveAllListeners();
        fpsButton.onClick.AddListener(() => AddSound());
        fpsButton.onClick.AddListener(() => FPSOn());

        saveScript.SaveSettings();
    }

    public void FPS()
    {
        if (fps)
        {
            FPSOn();
        }
        else
        {
            FPSOff();
        }
    }

    public void Music()
    {
        musicValue = musicSlider.value;
        musicText.text = musicValue.ToString();
        music.volume = musicValue / 10;

        saveScript.SaveSettings();
    }
    public void Sounds()
    {
        soundsValue = soundsSlider.value;
        soundsText.text = soundsValue.ToString();
        
        saveScript.SaveSettings();
    }
    public void SelectLanguage(bool first = false)
    {
        if (first == false)
        {
            languageIndex += 1;
        }

        if (languageIndex >= Enum.GetValues(typeof(Language)).Length)
        {
            languageIndex = 0;
        }
        languageManager.SetLanguage(languageIndex);

        if (languageIndex == 0)
        {
            languageText.text = "Русский";
        }
        else if (languageIndex == 1)
        {
            languageText.text = "English";
        }
        else if (languageIndex == 2)
        {
            languageText.text = "Indonesia";
        }

        UpdateTexts(languageManager.currentLanguage);

        languageButton.onClick.RemoveAllListeners();
        languageButton.onClick.AddListener(() => AddSound());
        languageButton.onClick.AddListener(() => SelectLanguage());

        saveScript.SaveSettings();
    }

    public void UpdateTexts(Language language)
    {
        if(language == Language.Russian)
        {
            if (qualityLevel == 0)
            {
                qualityText.text = "Оч. Низкое";
            }
            else if (qualityLevel == 1)
            {
                qualityText.text = "Низкое";
            }
            else if (qualityLevel == 2)
            {
                qualityText.text = "Среднее";
            }
            else if (qualityLevel == 3)
            {
                qualityText.text = "Высокое";
            }
            else if (qualityLevel == 4)
            {
                qualityText.text = "Оч. Высокое";
            }
            else if (qualityLevel == 5)
            {
                qualityText.text = "Ультра";
            }

            if(fps == true)
            {
                fpsText.text = "Вкл";
            }
            else
            {
                fpsText.text = "Выкл";
            }

            if(shadows == true)
            {
                shadowsText.text = "Вкл";
            }
            else
            {
                shadowsText.text = "Выкл";
            }
        }
        else if(language == Language.English)
        {
            if (qualityLevel == 0)
            {
                qualityText.text = "Very Low";
            }
            else if (qualityLevel == 1)
            {
                qualityText.text = "Low";
            }
            else if (qualityLevel == 2)
            {
                qualityText.text = "Medium";
            }
            else if (qualityLevel == 3)
            {
                qualityText.text = "High";
            }
            else if (qualityLevel == 4)
            {
                qualityText.text = "Very High";
            }
            else if (qualityLevel == 5)
            {
                qualityText.text = "Ultra";
            }

            if (fps == true)
            {
                fpsText.text = "On";
            }
            else
            {
                fpsText.text = "Off";
            }

            if (shadows == true)
            {
                shadowsText.text = "On";
            }
            else
            {
                shadowsText.text = "Off";
            }
        }
        else if (language == Language.Indonesian)
        {
            if (qualityLevel == 0)
            {
                qualityText.text = "Sangat Rendah";
            }
            else if (qualityLevel == 1)
            {
                qualityText.text = "Rendah";
            }
            else if (qualityLevel == 2)
            {
                qualityText.text = "Sedang";
            }
            else if (qualityLevel == 3)
            {
                qualityText.text = "Tinggi";
            }
            else if (qualityLevel == 4)
            {
                qualityText.text = "Sangat Tinggi";
            }
            else if (qualityLevel == 5)
            {
                qualityText.text = "Ultra";
            }

            if (fps == true)
            {
                fpsText.text = "Hidup";
            }
            else
            {
                fpsText.text = "Mati";
            }

            if (shadows == true)
            {
                shadowsText.text = "Hidup";
            }
            else
            {
                shadowsText.text = "Mati";
            }
        }
    }
}