using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;

    // Добавляем настройку для сохранения исходного состояния курсора
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    void Start()
    {
        pauseMenu.SetActive(false);

        // Проверка наличия EventSystem
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            Debug.LogError("EventSystem отсутствует в сцене! Добавьте его через GameObject > UI > EventSystem");
        }

        // Сохраняем начальное состояние курсора
        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Показываем курсор и разблокируем его при паузе
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Возвращаем курсор к предыдущему состоянию
        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousLockState;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        // Обеспечиваем видимость курсора в главном меню
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Проверяем существование сцены в настройках
        try
        {
            SceneManager.LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Не удалось загрузить сцену 'MainMenu'. Убедитесь, что сцена добавлена в Build Settings.");
            Debug.LogError("Ошибка: " + e.Message);
        }
    }
}
