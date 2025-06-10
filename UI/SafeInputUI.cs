using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SafeInputUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject codePanel;             
    public TMP_Text displayText;             
    
    [Header("Buttons")]
    public Button[] numberButtons;           
    public Button clearButton;               
    public Button enterButton;               
    public Button closeButton;               
    
    private string currentInput = "";        
    private const int CODE_LENGTH = 6;       
    private SafeController connectedSafe;    
    
    void Start()
    {
        SetupButtons();
        CloseCodePanel();
        PlayerController.EnableMouseLook();
    }
    
    void Update()
    {
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
            numberButtons[i].onClick.AddListener(() => {
                AddDigit(digit.ToString());
                if (connectedSafe != null)
                    connectedSafe.PlayButtonSound();
            });
        }
        
        // Настраиваем служебные кнопки
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(() => {
                ClearInput();
                if (connectedSafe != null)
                    connectedSafe.PlayButtonSound();
            });
        }
            
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(() => {
                SubmitCode();
                if (connectedSafe != null)
                    connectedSafe.PlayButtonSound();
            });
        }
            
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => {
                CloseCodePanel();
                if (connectedSafe != null)
                    connectedSafe.PlayButtonSound();
            });
        }
    }
    
    public void OpenCodePanel(SafeController safe)
    {
        connectedSafe = safe;
        codePanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.DisableMouseLook();
        
        ClearInput();
    }
    
    public void CloseCodePanel()
    {
        if (codePanel != null)
            codePanel.SetActive(false);
            
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    }
    
    void SubmitCode()
    {
        if (currentInput.Length == CODE_LENGTH && connectedSafe != null)
        {
            bool isCorrect = connectedSafe.TryCode(currentInput);
            
            if (isCorrect)
            {
                Invoke(nameof(CloseCodePanel), 1f);
            }
            else
            {
                ClearInput();
            }
        }
    }
    
    void UpdateDisplay()
    {
        if (displayText != null)
        {
            string display = currentInput.PadRight(CODE_LENGTH, '_');
            displayText.text = display;
        }
    }
}