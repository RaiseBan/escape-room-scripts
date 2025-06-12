using UnityEngine;

public class ClockController : MonoBehaviour
{
    [Header("Clock Settings")]
    public ClockColor clockColor;           // Цвет часов (настраиваете вручную)
    public Transform hourHand;              // Ссылка на HourHand

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
    private int displayedHour = 12;         // Какой час сейчас показывает стрелка

    void Start()
    {
        // Не меняем изначальное положение стрелки
        // Считаем что она показывает 12 часов
        displayedHour = 12;
    }

    /// <summary>
    /// Устанавливает час и плавно поворачивает стрелку
    /// </summary>
    public void SetHour(int hour)
    {
        // Ограничиваем от 1 до 9 (для цифр кода)
        hour = Mathf.Clamp(hour, 1, 9);

        if (!isRotating)
        {
            StartCoroutine(RotateToHour(hour));
        }
        else
        {
            RotateToHourInstant(hour);
        }
    }

    /// <summary>
    /// Мгновенно поворачивает стрелку к нужному часу
    /// </summary>
    public void SetHourInstant(int hour)
    {
        hour = Mathf.Clamp(hour, 1, 9);
        RotateToHourInstant(hour);
    }

    /// <summary>
    /// Возвращает текущий час как цифру для кода
    /// </summary>
    public int GetCurrentHour()
    {
        return displayedHour;
    }

    /// <summary>
    /// Возвращает цвет часов
    /// </summary>
    public ClockColor GetClockColor()
    {
        return clockColor;
    }

    /// <summary>
    /// Мгновенно поворачивает стрелку к нужному часу
    /// </summary>
    void RotateToHourInstant(int targetHour)
    {
        if (hourHand == null) return;

        // Вычисляем разность в часах
        int hourDifference = targetHour - displayedHour;
        
        // Конвертируем в градусы (каждый час = 30°)
        float angleToAdd = hourDifference * 30f;

        // Поворачиваем относительно текущего положения
        hourHand.Rotate(0, 0, angleToAdd);

        // Обновляем отображаемый час
        displayedHour = targetHour;

        Debug.Log($"Часы {clockColor}: повернуты на {angleToAdd}°, теперь показывают {displayedHour} час");
    }

    /// <summary>
    /// Плавно поворачивает стрелку к нужному часу
    /// </summary>
    System.Collections.IEnumerator RotateToHour(int targetHour)
    {
        if (hourHand == null) yield break;

        isRotating = true;

        Quaternion startRotation = hourHand.localRotation;

        // Вычисляем разность в часах
        int hourDifference = targetHour - displayedHour;
        
        // Конвертируем в градусы (каждый час = 30°)
        float angleToAdd = hourDifference * 30f;

        // Целевой поворот относительно текущего
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, angleToAdd);

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
        
        // Обновляем отображаемый час
        displayedHour = targetHour;
        
        isRotating = false;

        Debug.Log($"Часы {clockColor}: анимация завершена, повернуты на {angleToAdd}°, теперь показывают {displayedHour} час");
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

    [ContextMenu("Debug: Current Hour")]
    void DebugCurrentHour() 
    { 
        Debug.Log($"Часы {clockColor} сейчас показывают: {displayedHour} час"); 
    }
}