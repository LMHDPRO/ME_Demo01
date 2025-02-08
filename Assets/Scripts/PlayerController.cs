using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 12f;
    public float acceleration = 10f;
    public float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] public bool isDead;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private PlayerInput playerInput;
    private float targetSpeed;
    private float currentSpeed;
    private CheckpointSystem CheckpointSystem;
    private Transform playerTransform;
    private Animator animator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CheckpointSystem = GetComponent<CheckpointSystem>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        targetSpeed = walkSpeed;
        currentSpeed = walkSpeed;
        playerTransform = transform;
    }

    void Update()
    {
        // Verificar si el jugador está en el suelo
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, groundLayer);

        // Leer inputs del jugador en el InputManager
        moveInput = playerInput.actions["Horizontal"].ReadValue<Vector2>();

        if (!isDead)
        {
            FlipObject();
            // Cambiar entre caminar y correr de forma suave utilizando funciones de maths xd
            if (playerInput.actions["Sprint"].IsPressed() && isGrounded)
            {
                targetSpeed = sprintSpeed;
                
            }
            else
            {
                targetSpeed = walkSpeed;
            }
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

            animator.SetFloat("Speed", Mathf.Abs(moveInput.x) * currentSpeed);

            // Salto y su animación
            if (isGrounded && playerInput.actions["Jump"].WasPressedThisFrame())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            moveInput.x = 0;
        }
    }

    void FixedUpdate()
    {
        // Mover izquierda o derecha
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);
        
    }
    void FlipObject()
    {
        if (moveInput.x != 0)
        {
            // Voltear el objeto según la dirección izquierda o derecha
            playerTransform.localScale = new Vector3(-Mathf.Sign(moveInput.x)*1.5f, 1.5f, 1);
        }
    }
}
