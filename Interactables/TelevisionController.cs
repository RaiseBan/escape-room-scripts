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
    
    [Header("Emission Control")]
    public Color emissionColor = new Color(0.2f, 0.4f, 1f);
    public float maxEmissionIntensity = 3f;   
    public float minEmissionIntensity = 0.1f; 

    [Header("Sound Effects")]
    public AudioClip turnOnSound;        // Звук включения
    public AudioClip staticNoiseLoop;    // Шипение (looped)
    public AudioSource audioSource;      // AudioSource компонент

    [Header("Volume Settings")]
    public float turnOnVolume = 0.7f;        // Громкость звука включения
    public float staticNoiseVolume = 0.15f;  // Громкость шипения (тише)
    public float maxVolume = 1.0f;            // Максимальная громкость


    private bool isOn = false;
    private bool isAnimating = false;
    private Material screenMaterialInstance;

    void Start()
    {
        // Создаем AudioSource если нет
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Создаем копию материала
        if (tvScreenRenderer != null && tvOnMaterial != null)
        {
            screenMaterialInstance = new Material(tvOnMaterial);
            SetEmissionIntensity(0f);
        }
        
        UpdateTVDisplay();
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
        
        // Играем звук включения
        if (turnOnSound != null && audioSource != null)
        {
            audioSource.volume = turnOnVolume;  // Используем настраиваемую громкость
            audioSource.PlayOneShot(turnOnSound);
        }
        
        if (screenCanvas != null)
            screenCanvas.SetActive(false);

        if (tvScreenRenderer != null && screenMaterialInstance != null)
            tvScreenRenderer.material = screenMaterialInstance;

        // Эффект мерцания
        float elapsedTime = 0;
        
        while (elapsedTime < flickerDuration)
        {
            float randomIntensity = Random.Range(minEmissionIntensity, maxEmissionIntensity);
            SetEmissionIntensity(randomIntensity);
            
            elapsedTime += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // Окончательно включаем
        SetEmissionIntensity(maxEmissionIntensity);

        // Запускаем шипение после мерцания
        if (staticNoiseLoop != null && audioSource != null)
        {
            audioSource.clip = staticNoiseLoop;
            audioSource.loop = true;
            audioSource.volume = staticNoiseVolume;  // Используем отдельную настройку для шипения
            audioSource.Play();
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
        
        // Останавливаем все звуки
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        
        SetEmissionIntensity(0f);
        UpdateTVDisplay();
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
            if (isOn && screenMaterialInstance != null)
                tvScreenRenderer.material = screenMaterialInstance;
            else if (!isOn && tvOffMaterial != null)
                tvScreenRenderer.material = tvOffMaterial;
        }
    }

    void SetEmissionIntensity(float intensity)
    {
        if (screenMaterialInstance != null)
        {
            Color finalEmissionColor = emissionColor * intensity;
            screenMaterialInstance.SetColor("_EmissionColor", finalEmissionColor);
            
            if (intensity > 0)
                screenMaterialInstance.EnableKeyword("_EMISSION");
            else
                screenMaterialInstance.DisableKeyword("_EMISSION");
        }
    }

    void OnDestroy()
    {
        if (screenMaterialInstance != null)
        {
            DestroyImmediate(screenMaterialInstance);
        }
    }
}