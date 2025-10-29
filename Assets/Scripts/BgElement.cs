using UnityEngine;

public class BgElement : MonoBehaviour
{
    [Header("Background Element Settings")]
    public GameObject player; // Mantener esta referencia pública
    public float parallaxEffect = 0.5f;

    private Vector3 lastPlayerPosition;

    void Start()
    {
        // Buscar automáticamente al jugador si no está asignado
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("No se encontró ningún objeto con tag 'Player'. Asigna el tag 'Player' a tu personaje.");
                // Deshabilitar el script si no hay jugador
                enabled = false;
                return;
            }
        }

        lastPlayerPosition = player.transform.position;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 deltaMovement = player.transform.position - lastPlayerPosition;
        transform.position += deltaMovement * parallaxEffect;
        lastPlayerPosition = player.transform.position;
    }
}