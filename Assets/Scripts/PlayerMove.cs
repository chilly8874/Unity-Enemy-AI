using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private PlayerControls controls;
    Vector3 moveDirection;
    Rigidbody rb;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        moveDirection = Vector3.zero;

        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        moveDirection = controls.Player.Move.ReadValue<Vector3>();
    }
    private void FixedUpdate()
    {
        Vector3 direction = Vector3.ClampMagnitude(moveDirection, 1f);

    rb.linearVelocity = new Vector3(
        direction.x * moveSpeed,
        rb.linearVelocity.y,
        direction.z * moveSpeed
    );
    }
}
