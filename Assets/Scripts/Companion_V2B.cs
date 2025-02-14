using UnityEngine;

public class GhostFollower : MonoBehaviour
{
    public Transform player;
    public Transform companion;
    public float followSpeed = 3f;
    public float orbitSpeed = 55f;
    public float orbitRadius = 2f;
    public float checkRadius = 1f;
    public float heightOffset = 0.5f;
    public float checkHeight = 2f;
    public LayerMask obstacleLayer;

    private float angle = 0f;
    private Companion_V2 companionScript;
    public Rigidbody2D rb;

    void Start()
    {
        if (companion != null)
        {
            companionScript = companion.GetComponent<Companion_V2>();
        }
        
    }

    void Update()
    {
        if (player == null || companionScript == null) return;

        if (companionScript.isDead)
        {
            Vector3 safePosition = FindSafePositionAbovePlayer();
            MoveStraightToPlayer();
            if (safePosition != Vector3.zero)
            {
                companion.position = safePosition;
                Debug.Log("Teletransportando a zona segura en posicion " + safePosition);
            }
        }
        else
        {
            OrbitAroundPlayer();
        }
    }

    void OrbitAroundPlayer()
    {
        angle += orbitSpeed * Time.deltaTime;
        if (angle >= 360f) angle -= 360f;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
        Vector3 targetPosition = player.position + offset;
        targetPosition.y += heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    void MoveStraightToPlayer()
    {
        Vector2 targetPosition = player.position;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, followSpeed*4 * Time.deltaTime);
        if (rb == null) return;
        rb.MovePosition(newPosition);
        Debug.Log("Moviendo a compañero en linea recta hacia el jugador");
    }

    Vector3 FindSafePositionAbovePlayer()
    {
        Vector3 checkPosition = player.position;
        checkPosition.y += checkHeight;

        Debug.DrawRay(checkPosition, Vector2.up * checkRadius, Color.green, 0.1f);

        if (!CheckCollisionsAbove(checkPosition))
        {
            return checkPosition;
        }
        return Vector3.zero;
    }

    bool CheckCollisionsAbove(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);
        return hit != null;
    }


}
