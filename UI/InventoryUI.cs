using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Key Display")]
    public GameObject keySlot;           // Слот для ключа
    public Image keyIcon;                // Иконка ключа
    public TMP_Text keyText;             // Текст "Key: 1" или "Key: 0"

    void Start()
    {
        UpdateKeyDisplay();
    }

    public void UpdateKeyDisplay()
    {
        bool hasKey = GameManager.Instance.hasKey;

        // Показываем/скрываем иконку ключа
        if (keyIcon != null)
            keyIcon.gameObject.SetActive(hasKey);

        // Обновляем текст
        if (keyText != null)
        {
            keyText.text = hasKey ? "Key: 1" : "Key: 0";
        }

        // Можно также менять цвет слота
        if (keySlot != null)
        {
            Image slotImage = keySlot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.color = hasKey ? Color.yellow : Color.gray;
            }
        }
    }
}