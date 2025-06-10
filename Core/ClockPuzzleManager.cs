using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ClockPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public bool useClockPuzzle = true;           // Включить/выключить головоломку с часами

    [Header("Auto-find clocks")]
    public bool autoFindClocks = true;           // Автоматически найти все часы в сцене

    [Header("Manual clock assignment")]
    public ClockController[] manualClocks;       // Если не используем автопоиск

    private ClockController[] allClocks;
    private Dictionary<ClockController.ClockColor, int> colorToDigitMap;
    private ClockController.ClockColor[] codeSequence;
    private string generatedCode;

    void Start()
    {
        if (useClockPuzzle)
        {
            InitializeClocks();
            GenerateClockPuzzle();
        }
    }

    /// <summary>
    /// Инициализация всех часов в сцене
    /// </summary>
    void InitializeClocks()
    {
        if (autoFindClocks)
        {
            // Автоматически находим все часы в сцене
            allClocks = FindObjectsOfType<ClockController>();
        }
        else
        {
            // Используем вручную назначенные часы
            allClocks = manualClocks;
        }

        if (allClocks.Length == 0)
        {
            Debug.LogError("ClockPuzzleManager: Не найдено ни одних часов в сцене!");
            useClockPuzzle = false;
            return;
        }

        Debug.Log($"ClockPuzzleManager: Найдено {allClocks.Length} часов в сцене");

        // Проверяем что у нас есть часы всех нужных цветов
        var foundColors = allClocks.Select(c => c.GetClockColor()).Distinct().ToList();
        Debug.Log($"Найденные цвета часов: {string.Join(", ", foundColors)}");

        // Подробный список всех часов
        Debug.Log("=== Детальная информация о часах ===");
        for (int i = 0; i < allClocks.Length; i++)
        {
            var clock = allClocks[i];
            Debug.Log($"Часы #{i + 1}: {clock.name} - Цвет: {clock.GetClockColor()} - HourHand: {(clock.hourHand != null ? "OK" : "НЕ НАСТРОЕН!")}");
        }
    }

    /// <summary>
    /// Генерирует головоломку с часами
    /// </summary>
    void GenerateClockPuzzle()
    {
        if (!useClockPuzzle || allClocks.Length == 0) return;

        // 1. Генерируем 6-значный код
        generatedCode = GenerateRandomCode();

        // 2. Получаем доступные цвета часов
        var availableColors = allClocks.Select(c => c.GetClockColor()).Distinct().ToList();

        // Проверяем что у нас достаточно цветов
        if (availableColors.Count < 6)
        {
            Debug.LogError($"Недостаточно разных цветов часов! Найдено: {availableColors.Count}, нужно: 6");
            Debug.LogError($"Доступные цвета: {string.Join(", ", availableColors)}");
            Debug.LogError("Добавьте часы других цветов или используйте обычный режим без головоломки.");
            useClockPuzzle = false;
            return;
        }

        // 3. Создаем уникальную последовательность из 6 разных цветов
        var shuffledColors = new List<ClockController.ClockColor>(availableColors);

        // Перемешиваем список цветов
        for (int i = 0; i < shuffledColors.Count; i++)
        {
            var temp = shuffledColors[i];
            int randomIndex = Random.Range(i, shuffledColors.Count);
            shuffledColors[i] = shuffledColors[randomIndex];
            shuffledColors[randomIndex] = temp;
        }

        // Берем первые 6 цветов (все разные)
        codeSequence = shuffledColors.Take(6).ToArray();
        colorToDigitMap = new Dictionary<ClockController.ClockColor, int>();

        // 4. Назначаем каждому цвету соответствующую цифру из кода
        for (int i = 0; i < 6; i++)
        {
            int digit = int.Parse(generatedCode[i].ToString());
            colorToDigitMap[codeSequence[i]] = digit;
        }

        // 5. Устанавливаем часы на всех часах согласно маппингу
        SetAllClockTimes();

        // 6. Обновляем код в GameManager
        GameManager.Instance.safeCode = generatedCode;

        Debug.Log($"=== ГОЛОВОЛОМКА С ЧАСАМИ СОЗДАНА ===");
        Debug.Log($"Сгенерирован код: {generatedCode}");
        Debug.Log($"Последовательность цветов: {string.Join(" → ", codeSequence.Select(c => c.ToString()))}");
        Debug.Log($"Маппинг цвет→цифра: {string.Join(", ", colorToDigitMap.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
    }

    /// <summary>
    /// Генерирует случайный 6-значный код
    /// </summary>
    string GenerateRandomCode()
    {
        string code = "";
        for (int i = 0; i < 6; i++)
        {
            // Генерируем цифры от 1 до 9 (чтобы было удобно показывать на часах)
            code += Random.Range(1, 10).ToString();
        }
        Debug.Log($"Сгенерирован новый код: {code}");
        return code;
    }

    /// <summary>
    /// Устанавливает время на всех часах согласно маппингу
    /// </summary>
    void SetAllClockTimes()
    {
        Debug.Log("=== Устанавливаем время на часах ===");

        foreach (var clock in allClocks)
        {
            var clockColor = clock.GetClockColor();

            if (colorToDigitMap.ContainsKey(clockColor))
            {
                int hourToSet = colorToDigitMap[clockColor];
                clock.SetHourInstant(hourToSet);
                Debug.Log($"✅ Часы {clockColor} установлены на {hourToSet} час (используются в коде)");
            }
            else
            {
                // Если цвет не используется в коде, ставим случайное время
                int randomHour = Random.Range(1, 10);
                clock.SetHourInstant(randomHour);
                Debug.Log($"🔄 Часы {clockColor} установлены на {randomHour} час (не используются в коде)");
            }
        }
    }

    /// <summary>
    /// Возвращает последовательность цветов для показа на телевизоре
    /// </summary>
    public string GetColorSequenceText()
    {
        if (!useClockPuzzle || codeSequence == null)
            return "SAFE CODE:\n" + GameManager.Instance.safeCode;

        string colorText = "FIND THE CLOCKS IN ORDER:\n\n";

        for (int i = 0; i < codeSequence.Length; i++)
        {
            var clock = allClocks.FirstOrDefault(c => c.GetClockColor() == codeSequence[i]);
            if (clock != null)
            {
                colorText += $"{i + 1}. {clock.GetColorName()}\n";
            }
        }

        colorText += "\nRead the hour on each clock";
        return colorText;
    }

    /// <summary>
    /// Возвращает последовательность цветов как массив Color для UI
    /// </summary>
    public Color[] GetColorSequenceForUI()
    {
        if (!useClockPuzzle || codeSequence == null) return new Color[0];

        Color[] colors = new Color[codeSequence.Length];
        for (int i = 0; i < codeSequence.Length; i++)
        {
            var clock = allClocks.FirstOrDefault(c => c.GetClockColor() == codeSequence[i]);
            colors[i] = clock != null ? clock.GetUIColor() : Color.white;
        }

        return colors;
    }

    /// <summary>
    /// Проверка того, что головоломка активна
    /// </summary>
    public bool IsPuzzleActive()
    {
        return useClockPuzzle && allClocks != null && allClocks.Length > 0;
    }

    /// <summary>
    /// Получить правильный код
    /// </summary>
    public string GetCorrectCode()
    {
        return generatedCode;
    }

    // Метод для отладки - показать состояние всех часов
    [ContextMenu("Debug: Show Clock States")]
    public void DebugShowClockStates()
    {
        if (allClocks == null) return;

        Debug.Log("=== Состояние всех часов ===");
        foreach (var clock in allClocks)
        {
            Debug.Log($"{clock.GetColorName()} часы: {clock.GetCurrentHour()} час");
        }

        if (colorToDigitMap != null)
        {
            Debug.Log("=== Маппинг цвет → цифра ===");
            foreach (var kvp in colorToDigitMap)
            {
                Debug.Log($"{kvp.Key} → {kvp.Value}");
            }
        }

        Debug.Log($"Правильный код: {generatedCode}");
        Debug.Log($"Последовательность: {string.Join(" → ", codeSequence?.Select(c => c.ToString()) ?? new string[0])}");
    }
}