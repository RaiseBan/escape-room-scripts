using UnityEngine;
using TMPro;

public class TelevisionController : MonoBehaviour, IInteractable
{
    [Header("TV Screen")]
    public GameObject screenCanvas;      // Canvas с UI на экране телевизора
    public TMP_Text codeDisplayText;     // Текст для отображения кода

    [Header("TV Materials (Optional)")]
    public Renderer tvScreenRenderer;    // Рендерер экрана телевизора
    public Material tvOffMaterial;       // Материал выключенного экрана
    public Material tvOnMaterial;        // Материал включенного экрана

    private bool isOn = false;

    void Start()
    {
        UpdateTVDisplay();
    }

    public void Interact()
    {
        ToggleTV();
    }

    public string GetPromptText()
    {
        return isOn ? "Press E to turn off TV" : "Press E to turn on TV";
    }

    public bool CanInteract()
    {
        return true;  // Телевизор всегда можно включать/выключать
    }

    void ToggleTV()
    {
        isOn = !isOn;

        if (isOn)
        {
            GameManager.Instance.TurnOnTV();
        }
        else
        {
            GameManager.Instance.TurnOffTV();
        }

        UpdateTVDisplay();
    }

    void UpdateTVDisplay()
    {
        // Показываем/скрываем Canvas с кодом
        if (screenCanvas != null)
        {
            screenCanvas.SetActive(isOn);
        }

        // Обновляем текст кода
        if (codeDisplayText != null && isOn)
        {
            codeDisplayText.text = "SAFE CODE:\n" + GameManager.Instance.safeCode;
        }

        // Меняем материал экрана (если есть)
        if (tvScreenRenderer != null)
        {
            if (isOn && tvOnMaterial != null)
                tvScreenRenderer.material = tvOnMaterial;
            else if (!isOn && tvOffMaterial != null)
                tvScreenRenderer.material = tvOffMaterial;
        }
    }
}