using System.Collections;
using UnityEngine;

public class CheckPMarker : MonoBehaviour
{
    public GameObject Marker;
    public GameObject oldMarker;
    public Rigidbody2D Rigidbody2D;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bancho"))
        {
            //ACTIVA BANDERA Y DESAPARECE ANTIGUO CHECKPOINT
            Marker.SetActive(true);
            oldMarker.SetActive(false);
        }
    }
}
