using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public bool isTVOn = false;
    public string safeCode;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GenerateSafeCode();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void GenerateSafeCode()
    {
        // Генерируем 6-значный код
        safeCode = "";
        for (int i = 0; i < 6; i++)
        {
            safeCode += Random.Range(0, 10).ToString();
        }
        
        Debug.Log("Сгенерирован код для сейфа: " + safeCode);
    }
    
    public void TurnOnTV()
    {
        isTVOn = true;
        Debug.Log("Телевизор включен! Код на экране: " + safeCode);
    }
    
    public void TurnOffTV()
    {
        isTVOn = false;
        Debug.Log("Телевизор выключен.");
    }
}