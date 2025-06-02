using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SafeInputUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject codePanel;             // Панель ввода кода
    public TMP_Text displayText;             // Поле отображения введенного кода
    public TMP_Text feedbackText;            // Текст обратной связи (ошибки/успех)
    
    [Header("Buttons")]
    public Button[] numberButtons;           // Кнопки цифр 0-9
    public Button clearButton;               // Кнопка очистки
    public Button enterButton;               // Кнопка подтверждения
    public Button closeButton;               // Кнопка закрытия панели
    
    private string currentInput = "";        // Текущий введенный код
    private const int CODE_LENGTH = 6;       // Длина кода
    private SafeController connectedSafe;    // Ссылка на сейф
    
    void Start()
    {
        SetupButtons();
        CloseCodePanel();
        
        // Убеждаемся что поворот камеры включен при старте
        PlayerController.EnableMouseLook();
    }
    
    void Update()
    {
        // Закрытие панели по нажатию Escape
        if (codePanel != null && codePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCodePanel();
        }
    }
    
    void SetupButtons()
    {
        // Настраиваем кнопки цифр (0-9)
        for (int i = 0; i < numberButtons.Length && i < 10; i++)
        {
            int digit = i; // Важно для замыкания
            numberButtons[i].onClick.AddListener(() => AddDigit(digit.ToString()));
        }
        
        // Настраиваем служебные кнопки
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearInput);
            
        if (enterButton != null)
            enterButton.onClick.AddListener(SubmitCode);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCodePanel);
    }
    
    public void OpenCodePanel(SafeController safe)
    {
        connectedSafe = safe;
        codePanel.SetActive(true);
        
        // Разблокируем курсор для UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // БЛОКИРУЕМ поворот камеры
        PlayerController.DisableMouseLook();
        
        // Очищаем ввод
        ClearInput();
        
        if (feedbackText != null)
            feedbackText.text = "Enter 6-digit code";
    }
    
    public void CloseCodePanel()
    {
        if (codePanel != null)
            codePanel.SetActive(false);
            
        // Блокируем курсор обратно
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // ВКЛЮЧАЕМ поворот камеры обратно
        PlayerController.EnableMouseLook();
        
        connectedSafe = null;
    }
    
    void AddDigit(string digit)
    {
        if (currentInput.Length < CODE_LENGTH)
        {
            currentInput += digit;
            UpdateDisplay();
        }
    }
    
    void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
        
        if (feedbackText != null)
            feedbackText.text = "Enter 6-digit code";
    }
    
    void SubmitCode()
    {
        if (currentInput.Length == CODE_LENGTH && connectedSafe != null)
        {
            bool isCorrect = connectedSafe.TryCode(currentInput);
            
            if (isCorrect)
            {
                if (feedbackText != null)
                    feedbackText.text = "CORRECT! Safe opened!";
                    
                // Закрываем панель через секунду
                Invoke(nameof(CloseCodePanel), 1f);
            }
            else
            {
                if (feedbackText != null)
                    feedbackText.text = "WRONG CODE! Try again.";
                    
                ClearInput();
            }
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Enter complete 6-digit code!";
        }
    }
    
    void UpdateDisplay()
    {
        if (displayText != null)
        {
            // Показываем введенные цифры и подчеркивания для оставшихся
            string display = currentInput.PadRight(CODE_LENGTH, '_');
            displayText.text = display;
        }
    }
}