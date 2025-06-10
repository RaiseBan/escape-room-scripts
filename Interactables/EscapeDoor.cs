using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EscapeDoor : MonoBehaviour, IInteractable
{
    [Header("Door State")]
    public bool isOpen = false;
    
    [Header("Door Animation")]
    public GameObject doorObject;            
    public Vector3 openRotation = new Vector3(0, 90, 0);  
    public float openSpeed = 1.5f;           
    
    [Header("Victory Screen")]
    public GameObject victoryCanvas;         // Canvas с экраном победы
    public TMP_Text victoryText;             // Текст "YOU ESCAPED!"
    public Image fadeOverlay;                // Черный экран для затухания
    public Button menuButton;                // Кнопка "Return to Menu"
    
    [Header("Timing")]
    public float fadeToBlackDuration = 2f;   // Время затухания экрана
    public float victoryDisplayTime = 3f;    // Время показа текста победы
    public float autoReturnTime = 8f;        // Автоматический возврат в меню
    
    [Header("Sound Effects")]
    public AudioClip creakSound;             // Звук скрипа двери
    public AudioClip victorySound;           // Звук победы
    public AudioSource audioSource;          // AudioSource компонент
    
    void Start()
    {
        // Создаем AudioSource если нет
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Скрываем экран победы в начале
        if (victoryCanvas != null)
            victoryCanvas.SetActive(false);
            
        // Настраиваем кнопку меню
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMainMenu);
        }
    }
    
    public string GetPromptText()
    {
        if (GameManager.Instance.hasKey)
            return "Press E to escape!";
        else
            return "Door is locked. Need a key...";
    }
    
    public bool CanInteract()
    {
        return true; // Всегда можно взаимодействовать
    }
    
    public void Interact()
    {
        if (!isOpen && GameManager.Instance.hasKey)
        {
            // Открываем дверь и сразу сбегаем
            OpenDoor();
        }
        else if (!GameManager.Instance.hasKey)
        {
            Debug.Log("Дверь заперта! Нужен ключ.");
        }
    }
    
    void OpenDoor()
    {
        isOpen = true;
        
        // Играем звук скрипа в начале открытия
        if (creakSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(creakSound);
        }
        
        if (doorObject != null)
        {
            StartCoroutine(RotateDoor());
        }
        
        Debug.Log("Дверь открыта! Нажмите E чтобы выйти.");
    }
    
    void EscapeGame()
    {
        Debug.Log("Игрок сбежал! Показываем экран победы.");
        StartCoroutine(VictorySequence());
    }
    
    IEnumerator RotateDoor()
    {
        Quaternion startRotation = doorObject.transform.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(openRotation);
        float time = 0;
        
        while (time < 1)
        {
            time += Time.deltaTime * openSpeed;
            
            // Используем smooth curve для более естественного движения
            float smoothTime = Mathf.SmoothStep(0, 1, time);
            doorObject.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, smoothTime);
            
            yield return null;
        }
        
        doorObject.transform.localRotation = targetRotation;
        Debug.Log("Дверь полностью открыта!");
        
        // Небольшая пауза, затем автоматически сбегаем
        yield return new WaitForSeconds(0.5f);
        EscapeGame();
    }
    
    IEnumerator VictorySequence()
    {
        // Блокируем управление игроком
        PlayerController.DisableMouseLook();
        
        // 1. Показываем Canvas но скрываем текст и кнопку
        if (victoryCanvas != null)
            victoryCanvas.SetActive(true);
            
        // Скрываем текст и кнопку в начале
        if (victoryText != null)
            victoryText.gameObject.SetActive(false);
        if (menuButton != null)
            menuButton.gameObject.SetActive(false);
            
        // Убеждаемся что overlay прозрачный
        if (fadeOverlay != null)
        {
            Color overlayColor = fadeOverlay.color;
            overlayColor.a = 0f;
            fadeOverlay.color = overlayColor;
        }
        
        // 2. СНАЧАЛА плавное затухание экрана в черный
        yield return StartCoroutine(FadeToBlack());
        
        // 3. ПОТОМ показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 4. ПОТОМ играем звук победы
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
        
        // 5. ПОТОМ показываем текст победы
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
            victoryText.text = "YOU ESCAPED!";
        }
        
        // 6. Небольшая задержка и показываем кнопку
        yield return new WaitForSeconds(1f);
        
        if (menuButton != null)
            menuButton.gameObject.SetActive(true);
        
        // 7. Ждем некоторое время, затем автоматически возвращаемся в меню
        yield return new WaitForSeconds(autoReturnTime - 1f); // -1f потому что уже ждали 1 сек
        
        // Автоматический возврат, если игрок не нажал кнопку
        GoToMainMenu();
    }
    
    IEnumerator FadeToBlack()
    {
        if (fadeOverlay == null) yield break;
        
        float time = 0;
        Color startColor = fadeOverlay.color;
        Color targetColor = new Color(0, 0, 0, 1); // Черный
        
        while (time < fadeToBlackDuration)
        {
            time += Time.deltaTime;
            float progress = time / fadeToBlackDuration;
            
            fadeOverlay.color = Color.Lerp(startColor, targetColor, progress);
            yield return null;
        }
        
        fadeOverlay.color = targetColor;
    }
    
    public void GoToMainMenu()
    {
        // Останавливаем все корутины
        StopAllCoroutines();
        
        // Возвращаем нормальное состояние
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Переходим в главное меню
        SceneManager.LoadScene("MainMenu");
    }
}