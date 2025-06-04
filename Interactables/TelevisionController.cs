using UnityEngine;
using TMPro;
using System.Collections;

public class TelevisionController : MonoBehaviour, IInteractable
{
    [Header("TV Screen")]
    public GameObject screenCanvas;      
    public TMP_Text codeDisplayText;     

    [Header("TV Materials")]
    public Renderer tvScreenRenderer;    
    public Material tvOffMaterial;       
    public Material tvOnMaterial;        

    [Header("Animation Settings")]
    public float flickerDuration = 1.5f;
    public float flickerSpeed = 0.1f;
    public float codeAppearDelay = 1f;
    
    [Header("Screen Lighting")]
    public Light[] screenLights;             // Массив источников света
    public Color screenLightColor = Color.cyan;
    public float lightIntensity = 1f;

    private bool isOn = false;
    private bool isAnimating = false;

    void Start()
    {
        UpdateTVDisplay();
        
        // Настраиваем все источники света
        SetupScreenLights();
    }

    void SetupScreenLights()
    {
        foreach (Light light in screenLights)
        {
            if (light != null)
            {
                light.color = screenLightColor;
                light.intensity = 0;
                light.enabled = false;
                light.range = 3f; // Меньший радиус для каждого источника
            }
        }
    }

    public void Interact()
    {
        if (!isAnimating)
        {
            ToggleTV();
        }
    }

    public string GetPromptText()
    {
        if (isAnimating)
            return "TV is turning on...";
        return isOn ? "Press E to turn off TV" : "Press E to turn on TV";
    }

    public bool CanInteract()
    {
        return !isAnimating;
    }

    void ToggleTV()
    {
        isOn = !isOn;

        if (isOn)
        {
            StartCoroutine(TurnOnAnimation());
        }
        else
        {
            TurnOffInstantly();
        }
    }

    IEnumerator TurnOnAnimation()
    {
        isAnimating = true;
        
        if (screenCanvas != null)
            screenCanvas.SetActive(false);

        // Эффект мерцания
        float elapsedTime = 0;
        bool isFlickering = true;
        
        while (elapsedTime < flickerDuration)
        {
            // Переключаем материал
            if (tvScreenRenderer != null)
            {
                tvScreenRenderer.material = isFlickering ? tvOnMaterial : tvOffMaterial;
            }
            
            // Мерцаем всеми источниками света
            foreach (Light light in screenLights)
            {
                if (light != null)
                {
                    light.enabled = isFlickering;
                    light.intensity = isFlickering ? Random.Range(0.3f, lightIntensity) : 0;
                }
            }
            
            isFlickering = !isFlickering;
            elapsedTime += flickerSpeed;
            
            yield return new WaitForSeconds(flickerSpeed);
        }

        // Окончательно включаем
        if (tvScreenRenderer != null)
            tvScreenRenderer.material = tvOnMaterial;
            
        // Включаем все источники света
        foreach (Light light in screenLights)
        {
            if (light != null)
            {
                light.enabled = true;
                light.intensity = lightIntensity;
            }
        }

        yield return new WaitForSeconds(codeAppearDelay);
        
        if (screenCanvas != null)
        {
            screenCanvas.SetActive(true);
            StartCoroutine(FadeInCode());
        }

        GameManager.Instance.TurnOnTV();
        isAnimating = false;
    }

    IEnumerator FadeInCode()
    {
        if (codeDisplayText != null)
        {
            codeDisplayText.text = "SAFE CODE:\n" + GameManager.Instance.safeCode;
            
            Color originalColor = codeDisplayText.color;
            Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            
            codeDisplayText.color = transparentColor;
            
            float duration = 0.8f;
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, originalColor.a, elapsedTime / duration);
                codeDisplayText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            codeDisplayText.color = originalColor;
        }
    }

    void TurnOffInstantly()
    {
        GameManager.Instance.TurnOffTV();
        UpdateTVDisplay();
        
        // Выключаем все источники света
        foreach (Light light in screenLights)
        {
            if (light != null)
            {
                light.enabled = false;
                light.intensity = 0;
            }
        }
    }

    void UpdateTVDisplay()
    {
        if (screenCanvas != null)
        {
            screenCanvas.SetActive(isOn);
        }

        if (codeDisplayText != null && isOn)
        {
            codeDisplayText.text = "SAFE CODE:\n" + GameManager.Instance.safeCode;
        }

        if (tvScreenRenderer != null)
        {
            if (isOn && tvOnMaterial != null)
                tvScreenRenderer.material = tvOnMaterial;
            else if (!isOn && tvOffMaterial != null)
                tvScreenRenderer.material = tvOffMaterial;
        }
    }
}