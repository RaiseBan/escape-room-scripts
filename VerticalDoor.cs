using UnityEngine;
using System.Collections;

public class VerticalDoor : MonoBehaviour, IDoor
{
    public float openHeight = 2.0f;
    public float moveSpeed = 2.0f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
    }

    public void Open()
    {
        if (isOpen) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        isOpen = true;
        moveCoroutine = StartCoroutine(MoveDoor(openPosition));
    }

    public void Close()
    {
        if (!isOpen) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        isOpen = false;
        moveCoroutine = StartCoroutine(MoveDoor(closedPosition));
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;
        }

        transform.position = targetPosition;
    }
}