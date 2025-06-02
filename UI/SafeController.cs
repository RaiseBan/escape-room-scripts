using UnityEngine;

public class SafeController : MonoBehaviour, IInteractable
{
    [Header("Safe State")]
    public bool isOpen = false;

    [Header("Safe Door Animation")]
    public GameObject safeDoor;              // Дверь сейфа
    public Vector3 openRotation = new Vector3(0, -90, 0);  // Поворот при открытии

    [Header("Contents")]
    public GameObject keyObject;             // Ключ внутри сейфа

    [Header("UI")]
    public SafeInputUI safeInputUI;          // UI панель ввода кода

    void Start()
    {
        // Найти UI компонент если не назначен
        if (safeInputUI == null)
            safeInputUI = FindObjectOfType<SafeInputUI>();

        // Скрыть ключ изначально (полностью деактивировать объект)
        if (keyObject != null)
            keyObject.SetActive(false);
    }

    public string GetPromptText()
    {
        return "Press E to enter code";
    }

    public bool CanInteract()
    {
        return !isOpen;  // Можно взаимодействовать только если сейф закрыт
    }

    public void Interact()
    {
        if (!isOpen && safeInputUI != null)
        {
            safeInputUI.OpenCodePanel(this);
        }
    }

    // Вызывается из SafeInputUI когда пользователь вводит код
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
            Debug.Log("Wrong code! Try again.");
            return false;
        }
    }

    void OpenSafe()
    {
        isOpen = true;
        GameManager.Instance.OpenSafe();

        // Простая анимация открытия двери
        if (safeDoor != null)
        {
            safeDoor.transform.localRotation = Quaternion.Euler(openRotation);
        }

        // Показать ключ
        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }

        Debug.Log("Safe opened! Key is now visible.");
    }
}