using System.Collections;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float minFollowDistance = 3f;
    [SerializeField] private float maxFollowDistance = 5f;
    [SerializeField] private float runDistance = 5.1f;
    [SerializeField] private float deathDistance = 16f;
    [SerializeField] private float reviveDistance = 2.5f;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpHorizontalForce = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpCheckDistance = 1.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D companionCollider;

    [SerializeField] private float respawnTime = 10f;

    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;
    private bool isDead;
    private bool isGrounded;

    private void Start()
    {
        //Busca todos los componentes en el objeto y asigna valores predefinidos
        rb = GetComponent<Rigidbody2D>();
        companionCollider = GetComponent<CapsuleCollider2D>();
        isDead = false;
        rb.gravityScale = 1f;
    }

    private void Update()
    {
        //Busca al jugador y cálcula la diferencia de distancia
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        rb.bodyType = RigidbodyType2D.Dynamic;
        // Estados de muerte y resurrección
        if (distanceToPlayer > deathDistance && !isDead)
        {
            HandleDeathState();
        }
        else if (distanceToPlayer <= reviveDistance && isDead)
        {
            HandleReviveState();
        }

        // Movimiento y salto (solo si no está muerto)
        if (!isDead && distanceToPlayer <= runDistance)
        {
            HandleMovement(distanceToPlayer);
        }
        else
        {
            HandleJump();
        }

        if (isDead)
        {
            HandleDeathMovement();
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            HandleMovement(Vector2.Distance(transform.position, player.position));
        }
    }

    private void HandleDeathMovement()
    {
        //AQUI SE LE AGREGA LA FUERZA AL RIGIDBODY, PERO AL NO TENER FÍSICAS MIENTRAS MUERTO SE REQUIERE UNA OPOSICIÓN A LAS FUERZAS
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 force = directionToPlayer * followSpeed / 3;
        rb.AddForce(force, ForceMode2D.Force);

        Vector2 oppositeForce = -rb.linearVelocity.normalized * 0.5f;
        rb.AddForce(oppositeForce, ForceMode2D.Force);
    }


    private void HandleDeathState()
    {
        if (!isDead)
        {
            isDead = true;
            //AL MORIR ESTE APAGA SUS SIMULACIONES DE FÍSICA
            StartCoroutine(StopAndGo());
            //LUEGO INTENTA REGRESAR AL JUGADOR, SI DENTRO DE 10 SEGUNDOS NO LO LOGRA
            StartCoroutine(RespawnAfterTime()); 
            //SE TELETRANSPORTA DONDE ESTÁ EL JUGADOR
        }
        /*
        companionCollider.enabled = false;
        rb.gravityScale = 0f;
        Debug.Log("Collider desactivado: " + !companionCollider.enabled);
        MakeTransparent();
        */
    }

    private IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnTime); // Espera 10 segundos

        if (isDead) 
        {
            RespawnWithPlayer(); //SE TELETRANSPORTA DONDE ESTÁ EL JUGADOR
        }
    }

    private void RespawnWithPlayer()
    {
        isDead = false;
        transform.position = player.position; //SE TELETRANSPORTA DONDE ESTÁ EL JUGADOR
        //SI LLEGA A DONDE ESTÁ EL JUGADOR AQUÍ RE-ACTIVA SUS PROPIEDADES ORIGINALES
        companionCollider.enabled = true;
        rb.gravityScale = 1f; 
        MakeOpaque();
    }

    private void HandleReviveState()
    {
        //SI LLEGA A DONDE ESTÁ EL JUGADOR AQUÍ RE-ACTIVA SUS PROPIEDADES ORIGINALES
        isDead = false;
        companionCollider.enabled = true;
        rb.gravityScale = 1f; // Reactiva la gravedad
        MakeOpaque();
    }

    private void HandleMovement(float distanceToPlayer)
    {
        Vector2 direction = (player.position - transform.position).normalized;

        //AQUI REVISA SUS CONDICIONES DE MOVIMIENTO EN CAMINAR, CORRER Y IDLE

        if (distanceToPlayer > maxFollowDistance && distanceToPlayer <= runDistance)
        {
            rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
        }
        else if (distanceToPlayer > minFollowDistance && distanceToPlayer <= maxFollowDistance)
        {
            rb.linearVelocity = new Vector2(direction.x * followSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleJump()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, direction, jumpCheckDistance, groundLayer);

        if (obstacleHit.collider != null && isGrounded)
        {
            // Agregar impulso en ambas direcciones (vertical y horizontal)
            rb.linearVelocity = new Vector2(direction.x * jumpForce * jumpHorizontalForce, jumpForce);
            StartCoroutine(ApplyJumpCorrection(direction));
            Debug.Log("salto companion");
        }
    }

    private IEnumerator ApplyJumpCorrection(Vector2 direction)
    {
        yield return new WaitForSeconds(0.5f); // Espera 0.5 segundos
        //APLICA IMPULSO HORIZONTAL
        rb.linearVelocity = new Vector2(direction.x * jumpHorizontalForce, rb.linearVelocity.y);
    }

    private void MakeTransparent()
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 0.5f; 
        spriteRenderer.color = spriteColor;
    }

    private void MakeOpaque()
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1f;
        spriteRenderer.color = spriteColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            //AL TOCAR UN OBJETO CON ESE TAG ACTIVA MUERTE
            HandleDeathState();
            Debug.Log("Muertepordaño");
        }
    }

    IEnumerator StopAndGo()
    {
        //VUELVE AL ACOMPAÑANTE STATICO, LUEGO REGRESA A SU ESTADO NORMAL PARA REVIVIR
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        companionCollider.enabled = false;
        rb.gravityScale = 0f;
        Debug.Log("Collider desactivado: " + !companionCollider.enabled);
        MakeTransparent();
    }

}