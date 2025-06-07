using UnityEngine;
using System.Collections;

public class EscapeDoor : MonoBehaviour, IInteractable
{
    [Header("Door State")]
    public bool isOpen = false;
    
    [Header("Door Animation")]
    public GameObject doorObject;            
    public Vector3 openRotation = new Vector3(0, 90, 0);  
    public float openSpeed = 1.5f;           
    
    [Header("Win Screen")]
    public GameObject winScreen;             

    [Header("Sound Effects")]
    public AudioClip creakSound;             // Звук скрипа двери
    public AudioSource audioSource;          // AudioSource компонент
    
    void Start()
    {
        // Создаем AudioSource если нет
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public string GetPromptText()
    {
        if (isOpen)
            return "Door is open - You escaped!";
        else if (GameManager.Instance.hasKey)
            return "Press E to open door with key";
        else
            return "Door is locked. Need a key...";
    }
    
    public bool CanInteract()
    {
        return !isOpen;
    }
    
    public void Interact()
    {
        if (!isOpen && GameManager.Instance.hasKey)
        {
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
        
        Debug.Log("Поздравляем! Вы сбежали из комнаты!");
        
        Invoke(nameof(ShowWinScreen), 2f);
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
    }
    
    void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        Debug.Log("Игра завершена! Показан экран победы.");
    }
}