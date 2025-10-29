using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración Básica")]
    public Transform target;
    public float smoothSpeed = 0.125f;

    [Header("Límites en X")]
    public float minX = -Mathf.Infinity;
    public float maxX = Mathf.Infinity;

    [Header("Seguimiento en Y")]
    public bool followY = true; // ✅ NUEVO: Controlar si sigue en Y
    public float minY = -Mathf.Infinity;
    public float maxY = Mathf.Infinity;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // ✅ Offset configurable

    private float fixedZ;

    void Start()
    {
        if (target != null)
        {
            fixedZ = transform.position.z;

            // ✅ Posicionar cámara inmediatamente en el target al inicio
            Vector3 startPosition = target.position + offset;
            startPosition.z = fixedZ;
            transform.position = startPosition;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // ✅ Calcular posición deseada CON offset
        Vector3 desiredPosition = target.position + offset;

        // ✅ Aplicar límites en X
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        // ✅ Controlar seguimiento en Y
        if (!followY)
        {
            desiredPosition.y = transform.position.y; // Mantener Y actual
        }
        else
        {
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // ✅ Mantener Z fijo
        desiredPosition.z = fixedZ;

        // ✅ Suavizar movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("✅ Player encontrado automáticamente");
        }
    }

    // ✅ Método para reposicionar cámara inmediatamente
    public void SnapToTarget()
    {
        if (target != null)
        {
            Vector3 snapPosition = target.position + offset;
            snapPosition.z = fixedZ;
            transform.position = snapPosition;
        }
    }

    // ✅ Dibujar Gizmos para debugging
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(target.position + offset, new Vector3(1, 1, 0));
        }
    }
}