using UnityEngine;

public interface IInteractable
{
    void Interact();               // Что происходит при нажатии E
    string GetPromptText();        // Текст подсказки ("Press E to turn on TV")
    bool CanInteract();           // Можно ли взаимодействовать
}