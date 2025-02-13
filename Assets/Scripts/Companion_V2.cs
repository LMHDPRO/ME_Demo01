using UnityEngine;

public class Companion_V2 : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float speed = 3f; // Velocidad de movimiento
    public float followDistance = 1f; // Distancia m�nima antes de moverse
    public float jumpForce = 5f; // Fuerza del salto
    public float reviveDistance = 2f;
    public LayerMask groundLayer; // Capa del suelo para detectar saltos
    public GameObject ghost; // Referencia al objeto Ghost

    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private bool isGrounded;
    public bool isDead;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
    }

    void Update()
    {
        if (player == null) return;
        MakeOpaque();
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceGhostToPlayer = Vector2.Distance(ghost.transform.position, player.position);
        if (!isDead){
            spriteRenderer.enabled = true;
            if (distanceToPlayer > followDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
            }
            else
            {

                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            // Verificar si est� en el suelo antes de saltar
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, groundLayer);

            // Intentar saltar si hay un obstaculo delante
            if (isGrounded && CheckForObstacle())
            {
                Jump();
            }
        }
        else{
            if(distanceGhostToPlayer < reviveDistance) {
                HandleReviveState();
            }
       
        
    }
    

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    bool CheckForObstacle()
    {
        // Dispara un rayo hacia adelante para detectar obst�culos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(rb.linearVelocity.x), 0.6f, groundLayer);
        return hit.collider != null;
    }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            //AL TOCAR UN OBJETO CON ESE TAG ACTIVA MUERTE
            Debug.Log("Muertepordaño");
            Die();
        }
    }
    public void Die()
    {
        // Teletransportar al Ghost a la posición del NPC
        if (ghost != null)
        {
            isDead = true;
            ghost.transform.position = transform.position;
            MakeTransparent();
        }
    }

    private void MakeTransparent()
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 0.00001f;
        spriteRenderer.color = spriteColor;
    }

    private void MakeOpaque()
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1f;
        spriteRenderer.color = spriteColor;
    }

    public void HandleReviveState()
    {
        //SI LLEGA A DONDE ESTÁ EL JUGADOR AQUÍ RE-ACTIVA SUS PROPIEDADES ORIGINALES
        isDead = false;
        MakeOpaque();
    }
}
