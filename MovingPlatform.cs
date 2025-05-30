using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("Движение платформы")]
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 1.0f;
    public bool autoStart = true;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.position;

        if (autoStart)
            StartMoving();
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                if (targetPosition == endPosition)
                    targetPosition = startPosition;
                else
                    targetPosition = endPosition;
            }
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        targetPosition = endPosition;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        if (startPosition != Vector3.zero && endPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPosition, endPosition);
            Gizmos.DrawSphere(startPosition, 0.2f);
            Gizmos.DrawSphere(endPosition, 0.2f);
        }
    }
}