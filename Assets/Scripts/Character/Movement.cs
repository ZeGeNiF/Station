using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    PlayerInput playerInput;
    InputAction moveAction;
    Animator animator;

    [SerializeField] float speed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] Transform cameraTransform;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        moveAction = playerInput.actions.FindAction("Move");

        if (cameraTransform == null && Camera.main != null)
        { 
            cameraTransform = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        UpdateAnimation();
    }

    void MovePlayer()
    {
        Vector3 input = moveAction.ReadValue<Vector3>();

        //направление движения относильно от камеры
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * input.z + right * input.x;
        direction = Vector3.ClampMagnitude(direction, 1f);

        //Движения персонажа
        transform.position += direction * speed * Time.deltaTime;

        //Поворот в направления движения
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
                );
        }
    }

    void UpdateAnimation()
    {
        Vector3 input = moveAction.ReadValue<Vector3>();
        input.y = 0f;

        float animationSpeed = input.sqrMagnitude > 0.01f ? 0.4f : 0f;

        animator.SetFloat(
            "Speed",
            animationSpeed,
            0.03f,
            Time.deltaTime
            );
    }
}
