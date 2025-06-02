using UnityEngine;

public class EscapeKey : MonoBehaviour, IInteractable
{
    [Header("Key Settings")]
    public bool isPickedUp = false;

    [Header("Visual Effects (Optional)")]
    public float rotationSpeed = 50f;        // Скорость вращения ключа
    public float bobSpeed = 2f;              // Скорость подпрыгивания
    public float bobHeight = 0.2f;           // Высота подпрыгивания

    private Vector3 startPosition;
    private InventoryUI inventoryUI;

    void Start()
    {
        startPosition = transform.position;
        inventoryUI = FindObjectOfType<InventoryUI>();

        // Убеждаемся что у ключа есть коллайдер нормального размера
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Если коллайдера нет - добавляем небольшой
            SphereCollider newCol = gameObject.AddComponent<SphereCollider>();
            newCol.radius = 0.5f; // Нормальный размер
        }
    }

    void Update()
    {
        if (!isPickedUp)
        {
            // Простая анимация: вращение и подпрыгивание
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }

    public string GetPromptText()
    {
        return "Press E to pick up key";
    }

    public bool CanInteract()
    {
        // Ключ можно взять только если: не подобран + сейф открыт + ключ активен в сцене
        return !isPickedUp && GameManager.Instance.isSafeOpen && gameObject.activeInHierarchy;
    }

    public void Interact()
    {
        if (!isPickedUp && GameManager.Instance.isSafeOpen)
        {
            PickupKey();
        }
    }

    void PickupKey()
    {
        isPickedUp = true;
        GameManager.Instance.PickupKey();

        // Обновляем инвентарь
        if (inventoryUI != null)
            inventoryUI.UpdateKeyDisplay();

        // Скрываем ключ из мира
        gameObject.SetActive(false);

        Debug.Log("Ключ подобран и добавлен в инвентарь!");
    }
}