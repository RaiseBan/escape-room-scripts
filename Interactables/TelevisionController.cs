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
    public AudioClip turnOnSound;        
    public AudioClip staticNoiseLoop;    
    public AudioSource audioSource;      

    [Header("Volume Settings")]
    public float turnOnVolume = 0.7f;        
    public float staticNoiseVolume = 0.15f;  
    public float maxVolume = 1.0f;            

    private bool isOn = false;
    private bool isAnimating = false;
    private Material screenMaterialInstance;
    private ClockPuzzleManager clockPuzzleManager;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (tvScreenRenderer != null && tvOnMaterial != null)
        {
            screenMaterialInstance = new Material(tvOnMaterial);
            SetEmissionIntensity(0f);
        }
        
        // Находим менеджер головоломки с часами
        clockPuzzleManager = FindObjectOfType<ClockPuzzleManager>();
        
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
        
        if (turnOnSound != null && audioSource != null)
        {
            audioSource.volume = turnOnVolume;
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

        SetEmissionIntensity(maxEmissionIntensity);

        if (staticNoiseLoop != null && audioSource != null)
        {
            audioSource.clip = staticNoiseLoop;
            audioSource.loop = true;
            audioSource.volume = staticNoiseVolume;
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
            // Определяем что показывать: головоломку с часами или обычный код
            string displayText = GetDisplayText();
            codeDisplayText.text = displayText;
            
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

    /// <summary>
    /// Определяет какой текст показывать на экране
    /// </summary>
    string GetDisplayText()
    {
        // Если есть менеджер головоломки с часами и она активна
        if (clockPuzzleManager != null && clockPuzzleManager.IsPuzzleActive())
        {
            Debug.Log("Показываем головоломку с часами на ТВ");
            return clockPuzzleManager.GetColorSequenceText();
        }
        else
        {
            // Обычный режим - показываем код напрямую
            Debug.Log("Показываем обычный код на ТВ: " + GameManager.Instance.safeCode);
            return "SAFE CODE:\n" + GameManager.Instance.safeCode;
        }
    }

    void TurnOffInstantly()
    {
        GameManager.Instance.TurnOffTV();
        
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
            codeDisplayText.text = GetDisplayText();
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