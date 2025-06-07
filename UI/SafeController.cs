using UnityEngine;

public class SafeController : MonoBehaviour, IInteractable
{
    [Header("Safe State")]
    public bool isOpen = false;

    [Header("Safe Door Animation")]
    public GameObject safeDoor;              
    public Vector3 openRotation = new Vector3(0, -90, 0);  

    [Header("Contents")]
    public GameObject keyObject;             

    [Header("UI")]
    public SafeInputUI safeInputUI;          

    [Header("Sound Effects")]
    public AudioClip buttonClickSound;      // Звук нажатия кнопки
    public AudioClip successSound;          // Звук успешного открытия
    public AudioClip errorSound;            // Звук ошибки
    public AudioSource audioSource;         // AudioSource компонент

    void Start()
    {
        // Создаем AudioSource если нет
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (safeInputUI == null)
            safeInputUI = FindObjectOfType<SafeInputUI>();

        if (keyObject != null)
            keyObject.SetActive(false);
    }

    public string GetPromptText()
    {
        return "Press E to enter code";
    }

    public bool CanInteract()
    {
        return !isOpen;
    }

    public void Interact()
    {
        if (!isOpen && safeInputUI != null)
        {
            safeInputUI.OpenCodePanel(this);
        }
    }

    // Метод для воспроизведения звука кнопки (вызывается из SafeInputUI)
    public void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Метод для воспроизведения звука ошибки
    public void PlayErrorSound()
    {
        if (errorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    public bool TryCode(string inputCode)
    {
        string correctCode = GameManager.Instance.safeCode;

        if (inputCode == correctCode)
        {
            OpenSafe();
            return true;
        }
        else
        {
            PlayErrorSound(); // Играем звук ошибки
            Debug.Log("Wrong code! Try again.");
            return false;
        }
    }

    void OpenSafe()
    {
        isOpen = true;
        GameManager.Instance.OpenSafe();

        // Играем звук успешного открытия
        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        // Плавная анимация открытия двери
        if (safeDoor != null)
        {
            StartCoroutine(AnimateSafeDoor());
        }

        Debug.Log("Safe opened! Key is now visible.");
    }

    System.Collections.IEnumerator AnimateSafeDoor()
    {
        if (safeDoor == null) yield break;

        Quaternion startRotation = safeDoor.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(openRotation);
        
        float duration = 1.2f; // Длительность анимации
        float elapsedTime = 0;

        // Плавное открытие с easing
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Используем smooth curve для естественного движения
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            safeDoor.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
            
            yield return null;
        }

        safeDoor.transform.localRotation = targetRotation;

        // Показываем ключ после окончания анимации
        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
    }
}