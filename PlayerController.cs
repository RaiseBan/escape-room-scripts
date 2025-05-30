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

    [Header("Camera")]
    public Transform cameraHolder;
    public float mouseSensitivity = 100f;

    private Rigidbody _rb;
    private float _xRotation;
    private float _nextJumpTime;
    private bool _isGrounded;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded && Time.time >= _nextJumpTime)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _nextJumpTime = Time.time + jumpCooldown;
                Debug.Log("Jump!");
            }
            else
            {
                if (!_isGrounded)
                    Debug.Log("Not grounded");
                else
                    Debug.Log("Jump on cooldown");
            }
        }

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

        _rb.linearVelocity = new Vector3(move.x * moveSpeed, _rb.linearVelocity.y, move.z * moveSpeed);
    }
}