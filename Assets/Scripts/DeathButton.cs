using UnityEngine;

public class DeathButton : MonoBehaviour
{
    public Companion_V2 companion;

    public void CompanionDeath()
    {
        companion.Die();
    }
}
