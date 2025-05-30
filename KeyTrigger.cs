using UnityEngine;

public class KeyTrigger : MonoBehaviour
{
    public MonoBehaviour doorComponent; // Ссылка на компонент двери (Door или VerticalDoor)
    private IDoor doorToOpen; // Интерфейс для работы с дверью

    private void Start()
    {
        // Получаем интерфейс IDoor из компонента
        if (doorComponent != null)
        {
            doorToOpen = doorComponent as IDoor;
            if (doorToOpen == null)
            {
                Debug.LogError("Компонент не реализует интерфейс IDoor!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player") && doorToOpen != null)
        {
            // Открываем дверь
            doorToOpen.Open();

            // Уничтожаем триггер после использования
            Destroy(gameObject);
        }
    }
}