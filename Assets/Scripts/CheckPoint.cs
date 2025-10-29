using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Configuración del Checkpoint")]
    public bool debugMode = true;
    public bool isSafeCheckpoint = true; // ✅ Si este checkpoint es seguro para hazards

    [Header("Apariencia")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;
    public Color unsafeColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private bool isActive = false;
    private Animator animator;

    private void Start()
    {
        // ✅ OBTENER COMPONENTES
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // ✅ CONFIGURAR APARIENCIA INICIAL
        if (spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }

        if (debugMode)
        {
            Debug.Log($"📍 Checkpoint inicializado - Posición: {transform.position}");
            Debug.Log($"🛡️ Es seguro: {isSafeCheckpoint}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            // ✅ ACTUALIZAR CHECKPOINT DEL PLAYERMANAGER (tu sistema original)
            PlayerManager.lastCheckPointPos = transform.position;

            // ✅ ACTIVAR VISUALMENTE EL CHECKPOINT
            if (!isActive)
            {
                ActivateCheckpoint();
            }

            // ✅ ACTUALIZAR CHECKPOINTS SEGUROS EN HAZARDS (si es seguro)
            if (isSafeCheckpoint)
            {
                UpdateSafeCheckpointsInHazards();
            }

            if (debugMode)
            {
                Debug.Log($"📍 Checkpoint activado en posición: {transform.position}");
                Debug.Log($"🛡️ Checkpoint seguro: {isSafeCheckpoint}");
            }
        }
    }

    private void ActivateCheckpoint()
    {
        isActive = true;

        // ✅ CAMBIAR COLOR SEGÚN SEGURIDAD
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isSafeCheckpoint ? activeColor : unsafeColor;
        }

        // ✅ ACTIVAR ANIMACIÓN SI EXISTE
        if (animator != null)
        {
            animator.SetBool("IsActive", true);
        }

        // ✅ EFECTO DE PARTÍCULAS (opcional)
        ParticleSystem particles = GetComponent<ParticleSystem>();
        if (particles != null && !particles.isPlaying)
        {
            particles.Play();
        }

        Debug.Log("✅ Checkpoint activado visualmente");
    }

    private void UpdateSafeCheckpointsInHazards()
    {
        // ✅ BUSCAR TODOS LOS HAZARDS EN LA ESCENA
        Hazard[] allHazards = FindObjectsOfType<Hazard>();

        foreach (Hazard hazard in allHazards)
        {
            // ✅ VERIFICAR SI EL HAZARD TIENE EL MÉTODO UpdateSafeCheckpoint
            if (hazard != null)
            {
                // Usar reflexión para ser compatible con diferentes versiones
                System.Type hazardType = hazard.GetType();
                var method = hazardType.GetMethod("UpdateSafeCheckpoint");

                if (method != null)
                {
                    // ✅ CORREGIDO: Convertir Vector3 a Vector2 explícitamente
                    Vector2 checkpointPos = transform.position;
                    method.Invoke(hazard, new object[] { checkpointPos });

                    if (debugMode)
                    {
                        Debug.Log($"✅ Hazard actualizado con checkpoint seguro en posición: {checkpointPos}");
                    }
                }
                else if (debugMode)
                {
                    Debug.LogWarning($"⚠️ Checkpoint: Hazard no tiene método UpdateSafeCheckpoint");
                }
            }
        }

        if (debugMode)
        {
            Debug.Log($"✅ {allHazards.Length} hazards actualizados con checkpoint seguro");
        }
    }

    // ✅ MÉTODO PARA CAMBIAR ESTADO DE SEGURIDAD
    public void SetSafeStatus(bool safe)
    {
        isSafeCheckpoint = safe;

        if (spriteRenderer != null && isActive)
        {
            spriteRenderer.color = safe ? activeColor : unsafeColor;
        }

        if (debugMode)
        {
            Debug.Log($"🛡️ Checkpoint marcado como: {(safe ? "SEGURO" : "NO SEGURO")}");
        }
    }

    // ✅ MÉTODO PARA RESETEAR CHECKPOINT
    public void ResetCheckpoint()
    {
        isActive = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }

        if (animator != null)
        {
            animator.SetBool("IsActive", false);
        }

        // ✅ DETENER PARTÍCULAS
        ParticleSystem particles = GetComponent<ParticleSystem>();
        if (particles != null && particles.isPlaying)
        {
            particles.Stop();
        }

        Debug.Log("🔄 Checkpoint reseteado");
    }

    // ✅ MÉTODO PARA VERIFICAR SI ESTE ES EL CHECKPOINT ACTIVO ACTUAL
    public bool IsCurrentCheckpoint()
    {
        return PlayerManager.lastCheckPointPos == (Vector2)transform.position;
    }

    // ✅ MÉTODOS DE DEBUG MANUAL
    [ContextMenu("📍 Activar Checkpoint Manualmente")]
    public void ActivarManualmente()
    {
        PlayerManager.lastCheckPointPos = transform.position;
        ActivateCheckpoint();

        if (isSafeCheckpoint)
        {
            UpdateSafeCheckpointsInHazards();
        }

        Debug.Log($"📍 Checkpoint activado manualmente: {transform.position}");
        Debug.Log($"🛡️ Es seguro: {isSafeCheckpoint}");
    }

    [ContextMenu("🛡️ Marcar como Seguro")]
    public void MarcarComoSeguro()
    {
        SetSafeStatus(true);
    }

    [ContextMenu("⚠️ Marcar como No Seguro")]
    public void MarcarComoNoSeguro()
    {
        SetSafeStatus(false);
    }

    [ContextMenu("🔍 Verificar Estado")]
    public void VerificarEstado()
    {
        Debug.Log("🔍 CHECKPOINT - Estado actual:");
        Debug.Log($"📍 Posición: {transform.position}");
        Debug.Log($"🎯 Activo: {isActive}");
        Debug.Log($"🛡️ Seguro: {isSafeCheckpoint}");
        Debug.Log($"📌 Es checkpoint actual: {IsCurrentCheckpoint()}");
        Debug.Log($"📌 Checkpoint global: {PlayerManager.lastCheckPointPos}");
    }

    [ContextMenu("🔄 Resetear Checkpoint")]
    public void ResetearManual()
    {
        ResetCheckpoint();
    }

    // ✅ DIBUJAR GIZMOS PARA VISUALIZAR EN EDITOR
    private void OnDrawGizmosSelected()
    {
        // ✅ DIBUJAR ICONO SEGÚN SEGURIDAD
        if (isSafeCheckpoint)
        {
            Gizmos.color = Color.green;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, "📍 SAFE CHECKPOINT");
#endif
        }
        else
        {
            Gizmos.color = Color.yellow;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, "⚠️ UNSAFE CHECKPOINT");
#endif
        }

        // ✅ DIBUJAR ÁREA DEL CHECKPOINT
        Gizmos.color = isSafeCheckpoint ?
            new Color(0, 1, 0, 0.2f) : // Verde semitransparente para seguro
            new Color(1, 1, 0, 0.2f);  // Amarillo semitransparente para no seguro

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // ✅ DIBUJAR SIEMPRE UN PEQUEÑO ICONO
        if (IsCurrentCheckpoint() && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}