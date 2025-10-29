using UnityEngine;

public class SimpleDeathPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("💀 DeathPlane: Jugador cayó - muerte instantánea");

            // Buscar HealthManager y matar
            HealthManager health = collision.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(999); // Daño mortal
            }
        }
    }
}
