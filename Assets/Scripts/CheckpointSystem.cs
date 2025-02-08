using System.Collections;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition;
    public float respawnDelay = 3f;
    private PlayerController playerController;
    public GameObject checkpointLevel;
    public CapsuleCollider2D capsuleCollider;
    private Animator animator;

    void Start()
    {
        // Al inicio, la posición de respawn es la posición del primer checkpoint
        respawnPosition = checkpointLevel.transform.position;
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador toca un checkpoint, almacena la nueva posición
        if (collision.CompareTag("Checkpoint"))
        {
            respawnPosition = collision.transform.position;
            Debug.Log("Nueva posición para reaparecer" + respawnPosition.sqrMagnitude);
        }
        if (collision.CompareTag("Hazard")) 
        {
            // Si toca un trigger de peligro muere y aparece en el último checkpoint valido
            Debug.Log("Muere");
            animator.SetTrigger("Death");
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator RespawnPlayer()
    {
        playerController.isDead = true;
        animator.SetTrigger("Death");
        // Tiempo antes de reaparecer
        yield return new WaitForSeconds(2f);
        // Regresa al último checkpoint
        transform.position = respawnPosition;
        // Reactiva la movilidad del jugador y hace animación fachera
        playerController.isDead = false; 
        animator.SetTrigger("Recover");
    }
}
