using UnityEngine;

public class Companion_V2 : MonoBehaviour
{
    public GameObject player;
    public float speed = 3f;
    public float followDistance = 1f;
    public float deathDistance = 100f;
    public float jumpForce = 5f;
    public float reviveDistance = 2f;
    public LayerMask groundLayer;
    public GameObject ghost;

    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer ghostSpriteRenderer;
    private bool isGrounded;
    public bool isDead;

    private float groundCheckDistance = 1.2f;
    private float obstacleCheckDistance = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        Collider2D col1 = GetComponent<Collider2D>();
        Collider2D col2 = player.GetComponent<Collider2D>();

        if (col1 != null && col2 != null)
        {
            Physics2D.IgnoreCollision(col1, col2, true);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float distanceGhostToPlayer = Vector2.Distance(ghost.transform.position, player.transform.position);

        if (!isDead)
        {
            if (distanceToPlayer > followDistance)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            if (distanceToPlayer > deathDistance)
            {
                Die();
            }
            // Comprobación de suelo
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
            Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.red);

            // Detectar si hay un obstáculo enfrente
            if (isGrounded && CheckForObstacle())
            {
                Jump();
            }
        }
        else
        {
            if (distanceGhostToPlayer < reviveDistance)
            {
                HandleReviveState();
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    bool CheckForObstacle()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) return false;

        Vector2 direction = Vector2.right * Mathf.Sign(rb.linearVelocity.x);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, direction, obstacleCheckDistance, groundLayer);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, direction * obstacleCheckDistance, Color.blue);

        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            Debug.Log("Muerte por daño");
            Die();
        }
    }

    public void Die()
    {
        if (ghost != null)
        {
            isDead = true;
            ghost.transform.position = transform.position;
            HideSprite();
        }
    }

    private void HideSprite()
    {
        spriteRenderer.enabled = false;
        ghostSpriteRenderer.enabled = true;
    }

    private void MakeOpaque()
    {
        spriteRenderer.enabled = true;
        ghostSpriteRenderer.enabled = false;
    }

    public void HandleReviveState()
    {
        isDead = false;
        MakeOpaque();
    }
}
