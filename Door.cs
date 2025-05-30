using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IDoor
{
    public Vector3 hingePosition = Vector3.zero;
    public float openAngle = 90.0f;
    public float moveSpeed = 2.0f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    public void Open()
    {
        if (isOpen) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        isOpen = true;
        moveCoroutine = StartCoroutine(RotateDoor(openRotation));
    }

    public void Close()
    {
        if (!isOpen) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        isOpen = false;
        moveCoroutine = StartCoroutine(RotateDoor(closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float time = 0;

        Vector3 worldHingePos = transform.position + transform.TransformDirection(hingePosition);

        while (time < 1)
        {
            time += Time.deltaTime * moveSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            Vector3 newHingeWorldPos = transform.position + transform.TransformDirection(hingePosition);
            transform.position += worldHingePos - newHingeWorldPos;
            yield return null;
        }

        transform.rotation = targetRotation;
        Vector3 finalHingeWorldPos = transform.position + transform.TransformDirection(hingePosition);
        transform.position += worldHingePos - finalHingeWorldPos;
    }
}