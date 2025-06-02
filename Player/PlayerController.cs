using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 5f;
    public float jumpCooldown = 0.3f;
    public float groundCheckDistance = 1.2f;
    public LayerMask groundMask = ~0;

    [Header("Crouching")]
    public float crouchSpeed = 4f;           // Скорость при приседании
    public float crouchHeight = 0.5f;        // Высота при приседании
    public float crouchTransitionSpeed = 8f; // Скорость перехода в присед

    [Header("Camera")]
    public Transform cameraHolder;
    public float mouseSensitivity = 100f;

    private Rigidbody _rb;
    private float _xRotation;
    private float _nextJumpTime;
    private bool _isGrounded;

    // Crouching variables
    private bool _isCrouching = false;
    private Vector3 _originalCameraPosition;
    private Vector3 _crouchCameraPosition;

    // Mouse look control
    public static bool canLookAround = true;  // Статический флаг для блокировки поворота

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        // Запоминаем изначальную позицию камеры
        if (cameraHolder != null)
        {
            _originalCameraPosition = cameraHolder.localPosition;
            _crouchCameraPosition = _originalCameraPosition + Vector3.down * crouchHeight;
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleCrouching();
        HandleJumping();
        HandleMouseLook();
    }

    void HandleCrouching()
    {
        // Проверяем нажатие Ctrl или C для приседания
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        if (crouchInput && !_isCrouching)
        {
            StartCrouch();
        }
        else if (!crouchInput && _isCrouching)
        {
            StopCrouch();
        }

        // Плавно перемещаем камеру
        if (cameraHolder != null)
        {
            Vector3 targetPosition = _isCrouching ? _crouchCameraPosition : _originalCameraPosition;
            cameraHolder.localPosition = Vector3.Lerp(
                cameraHolder.localPosition,
                targetPosition,
                crouchTransitionSpeed * Time.deltaTime
            );
        }
    }

    void StartCrouch()
    {
        _isCrouching = true;
    }

    void StopCrouch()
    {
        _isCrouching = false;
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Нельзя прыгать при приседании
            if (_isGrounded && Time.time >= _nextJumpTime && !_isCrouching)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _nextJumpTime = Time.time + jumpCooldown;
            }
        }
    }

    void HandleMouseLook()
    {
        // Не поворачиваем камеру если движение мыши заблокировано
        if (!canLookAround) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void CheckGrounded()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);

        RaycastHit hit;
        _isGrounded = Physics.SphereCast(
            transform.position + Vector3.up * 0.5f,
            0.4f,
            Vector3.down,
            out hit,
            groundCheckDistance,
            groundMask
        );
    }

    void FixedUpdate()
    {
        Vector3 move = cameraHolder.forward * Input.GetAxis("Vertical") +
                     cameraHolder.right * Input.GetAxis("Horizontal");

        // Используем разную скорость в зависимости от того, приседает ли игрок
        float currentSpeed = _isCrouching ? crouchSpeed : moveSpeed;

        _rb.linearVelocity = new Vector3(move.x * currentSpeed, _rb.linearVelocity.y, move.z * currentSpeed);
    }

    // Методы для блокировки/разблокировки поворота камеры
    public static void DisableMouseLook()
    {
        canLookAround = false;
    }

    public static void EnableMouseLook()
    {
        canLookAround = true;
    }
}