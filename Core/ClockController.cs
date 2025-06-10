using UnityEngine;

public class ClockController : MonoBehaviour
{
    [Header("Clock Settings")]
    public ClockColor clockColor;           // Цвет часов (настраиваете вручную)
    public Transform hourHand;              // Ссылка на HourHand
    public int currentHour = 1;             // Текущий час (1-9)

    [Header("Animation")]
    public float rotationSpeed = 2f;        // Скорость поворота стрелки

    [Header("Debug (Optional)")]
    public Transform[] hourMarkers;         // Массив засечек 1h-12h для отладки

    public enum ClockColor
    {
        Blue,       // Синий
        Pink,       // Розовый  
        Green,      // Зеленый
        Brown,      // Коричневый
        Purple,     // Фиолетовый
        Gray        // Серый
    }

    private bool isRotating = false;

    void Start()
    {
        // Устанавливаем изначальное положение стрелки
        SetHourInstant(currentHour);
    }

    /// <summary>
    /// Устанавливает час и плавно поворачивает стрелку
    /// </summary>
    public void SetHour(int hour)
    {
        // Ограничиваем от 1 до 9 (для цифр кода)
        currentHour = Mathf.Clamp(hour, 1, 9);

        if (!isRotating)
        {
            StartCoroutine(RotateToHour());
        }
        else
        {
            UpdateHandPosition();
        }
    }

    /// <summary>
    /// Мгновенно устанавливает положение стрелки без анимации
    /// </summary>
    public void SetHourInstant(int hour)
    {
        currentHour = Mathf.Clamp(hour, 1, 9);
        UpdateHandPosition();
    }

    /// <summary>
    /// Возвращает текущий час как цифру для кода
    /// </summary>
    public int GetCurrentHour()
    {
        return currentHour;
    }

    /// <summary>
    /// Возвращает цвет часов
    /// </summary>
    public ClockColor GetClockColor()
    {
        return clockColor;
    }

    /// <summary>
    /// Обновляет положение стрелки без анимации
    /// </summary>
    void UpdateHandPosition()
    {
        if (hourHand == null) return;

        // Правильный расчет угла для часовой стрелки
        // 12 часов = 0° (вверх), 1 час = 30°, 2 часа = 60°, и т.д.
        // Но так как мы показываем часы 1-9, нужно корректно рассчитать

        float angle;
        if (currentHour == 12)
        {
            angle = 0f; // 12 часов - стрелка вверх
        }
        else
        {
            angle = currentHour * 30f; // 1 час = 30°, 2 часа = 60°, и т.д.
        }

        // В Unity поворот Z по часовой стрелке = отрицательный угол
        hourHand.localRotation = Quaternion.Euler(0, 0, -angle);

        Debug.Log($"Часы {clockColor}: установлены на {currentHour} час, угол {-angle}°");
    }

    /// <summary>
    /// Плавно поворачивает стрелку к нужному часу
    /// </summary>
    System.Collections.IEnumerator RotateToHour()
    {
        if (hourHand == null) yield break;

        isRotating = true;

        Quaternion startRotation = hourHand.localRotation;

        float targetAngle;
        if (currentHour == 12)
        {
            targetAngle = 0f;
        }
        else
        {
            targetAngle = currentHour * 30f;
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, -targetAngle);

        float elapsedTime = 0f;
        float duration = 1f / rotationSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Плавная интерполяция с easing
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);
            hourHand.localRotation = Quaternion.Slerp(startRotation, targetRotation, smoothProgress);

            yield return null;
        }

        hourHand.localRotation = targetRotation;
        isRotating = false;

        Debug.Log($"Часы {clockColor}: анимация завершена, указывают на {currentHour} час");
    }

    /// <summary>
    /// Получить название цвета для отображения
    /// </summary>
    public string GetColorName()
    {
        switch (clockColor)
        {
            case ClockColor.Blue: return "BLUE";
            case ClockColor.Pink: return "PINK";
            case ClockColor.Green: return "GREEN";
            case ClockColor.Brown: return "BROWN";
            case ClockColor.Purple: return "PURPLE";
            case ClockColor.Gray: return "GRAY";
            default: return "UNKNOWN";
        }
    }

    /// <summary>
    /// Получить цвет для UI
    /// </summary>
    public Color GetUIColor()
    {
        switch (clockColor)
        {
            case ClockColor.Blue: return Color.blue;
            case ClockColor.Pink: return Color.magenta;
            case ClockColor.Green: return Color.green;
            case ClockColor.Brown: return new Color(0.6f, 0.3f, 0.1f);
            case ClockColor.Purple: return new Color(0.5f, 0f, 0.5f);
            case ClockColor.Gray: return Color.gray;
            default: return Color.white;
        }
    }

    // Методы для отладки
    [ContextMenu("Test: Set to Hour 1")]
    void TestHour1() { SetHour(1); }

    [ContextMenu("Test: Set to Hour 5")]
    void TestHour5() { SetHour(5); }

    [ContextMenu("Test: Set to Hour 9")]
    void TestHour9() { SetHour(9); }
}