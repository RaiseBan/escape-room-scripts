using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 4f;  // Увеличиваем общее расстояние взаимодействия
    public Camera playerCamera;
    
    [Header("UI References")]
    public GameObject promptUI;          // PromptUI GameObject
    public TMP_Text promptText;          // PromptText компонент
    
    private IInteractable currentInteractable;
    
    void Start()
    {
        // Если камера не назначена, ищем Camera на этом же объекте или его детях
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        
        // Скрываем подсказку в начале
        if (promptUI != null)
            promptUI.SetActive(false);
    }
    
    void Update()
    {
        CheckForInteractable();
        HandleInput();
    }
    
    void CheckForInteractable()
    {
        // Raycast от центра экрана
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                ShowPrompt(interactable);
                currentInteractable = interactable;
                return;
            }
        }
        
        // Если ничего не найдено
        HidePrompt();
        currentInteractable = null;
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    
    void ShowPrompt(IInteractable interactable)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(true);
            if (promptText != null)
                promptText.text = interactable.GetPromptText();
        }
    }
    
    void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}