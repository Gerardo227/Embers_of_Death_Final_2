using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    [Header("Configuración del Hazard")]
    public bool debugMode = true;
    public Vector2 respawnOffset = new Vector2(0f, 2f);
    public int damageAmount = 1;

    [Header("Configuración de Capas")]
    public bool forceSpecificLayer = true;
    public int targetLayer = 2; // Level 2

    [Header("Sistema de Renderizado")]
    public bool forcePlayerVisibility = true;
    public bool enableImmunityEffect = false;
    public float immunityDuration = 1f;

    private Vector2 safeCheckpointPosition;
    private bool isRespawning = false;

    private void Start()
    {
        // Buscar el player al inicio
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            safeCheckpointPosition = player.transform.position;
            if (debugMode)
            {
                Debug.Log("✅ Hazard inicializado correctamente");
                Debug.Log("📍 Posición segura inicial: " + safeCheckpointPosition);
                Debug.Log("🎯 Player encontrado: " + player.name);
                Debug.Log("👀 Sistema de renderizado forzado: " + forcePlayerVisibility);
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró el objeto con tag 'Player'");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (debugMode) Debug.Log("🎯 OnTriggerEnter2D activado con: " + collision.gameObject.name);

        if (collision.CompareTag("Player") && !isRespawning)
        {
            if (debugMode) Debug.Log("💀 Player detectado en el hazard - Iniciando proceso");

            // ✅ ORDEN CRÍTICO: PRIMERO FORZAR VISIBILIDAD, LUEGO EL RESTO
            if (forcePlayerVisibility)
            {
                ForcePlayerVisibility(collision.gameObject);
            }

            // ✅ APLICAR DAÑO
            ApplyDamageToPlayer(collision.gameObject);

            // ✅ HACER RESPAWN
            StartCoroutine(RespawnPlayerCoroutine(collision.gameObject));
        }
        else
        {
            if (debugMode) Debug.Log("⚠️ Objeto no es Player o ya está en respawn: " + collision.gameObject.name);
        }
    }

    // ✅ MÉTODO CRÍTICO: FORZAR VISIBILIDAD DEL PLAYER
    private void ForcePlayerVisibility(GameObject player)
    {
        if (debugMode) Debug.Log("🔆 Iniciando ForcePlayerVisibility");

        int renderersFixed = 0;
        int materialsFixed = 0;

        // 1. BUSCAR TODOS LOS RENDERERS EN EL PLAYER Y SUS HIJOS
        Renderer[] allRenderers = player.GetComponentsInChildren<Renderer>(true);
        if (debugMode) Debug.Log("🔍 Encontrados " + allRenderers.Length + " componentes Renderer");

        foreach (Renderer renderer in allRenderers)
        {
            if (renderer != null)
            {
                // Asegurar que el renderer esté activado
                if (!renderer.enabled)
                {
                    renderer.enabled = true;
                    if (debugMode) Debug.Log("✅ Renderer activado: " + renderer.name);
                }

                // Reparar todos los materiales del renderer
                foreach (Material material in renderer.materials)
                {
                    if (material != null)
                    {
                        Color currentColor = material.color;
                        if (currentColor.a < 0.99f)
                        {
                            material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                            materialsFixed++;
                            if (debugMode) Debug.Log("🎨 Material reparado - Alpha forzado a 1: " + material.name);
                        }
                    }
                }
                renderersFixed++;
            }
        }

        // 2. BUSCAR SPRITE RENDERERS ESPECÍFICAMENTE
        SpriteRenderer[] spriteRenderers = player.GetComponentsInChildren<SpriteRenderer>(true);
        if (debugMode) Debug.Log("🔍 Encontrados " + spriteRenderers.Length + " componentes SpriteRenderer");

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                // Forzar visibilidad del sprite renderer
                spriteRenderer.enabled = true;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

                if (debugMode) Debug.Log("✅ SpriteRenderer forzado a visible: " + spriteRenderer.name);
            }
        }

        // 3. BUSCAR MESH RENDERERS
        MeshRenderer[] meshRenderers = player.GetComponentsInChildren<MeshRenderer>(true);
        if (debugMode) Debug.Log("🔍 Encontrados " + meshRenderers.Length + " componentes MeshRenderer");

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                foreach (Material material in meshRenderer.materials)
                {
                    if (material.HasProperty("_Color"))
                    {
                        Color color = material.color;
                        material.color = new Color(color.r, color.g, color.b, 1f);
                    }
                }
            }
        }

        if (debugMode)
        {
            Debug.Log("🔆 ForcePlayerVisibility completado:");
            Debug.Log("   └── Renderers reparados: " + renderersFixed);
            Debug.Log("   └── Materials reparados: " + materialsFixed);
            Debug.Log("   └── Total componentes revisados: " + (allRenderers.Length + spriteRenderers.Length + meshRenderers.Length));
        }
    }

    // ✅ MÉTODO PARA APLICAR DAÑO
    private void ApplyDamageToPlayer(GameObject player)
    {
        if (debugMode) Debug.Log("❤️ Iniciando aplicación de daño: " + damageAmount);

        // OPCIÓN 1: Buscar HealthManager en el player
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            if (debugMode) Debug.Log("✅ HealthManager encontrado en el Player");
            healthManager.TakeDamage(damageAmount);
            if (debugMode) Debug.Log("✅ Daño aplicado via HealthManager component");
            return;
        }

        if (debugMode) Debug.Log("⚠️ HealthManager no encontrado en Player, buscando en escena...");

        // OPCIÓN 2: Buscar HealthManager en cualquier objeto de la escena
        HealthManager healthManagerInScene = FindObjectOfType<HealthManager>();
        if (healthManagerInScene != null)
        {
            if (debugMode) Debug.Log("✅ HealthManager encontrado en la escena");
            healthManagerInScene.TakeDamage(damageAmount);
            if (debugMode) Debug.Log("✅ Daño aplicado via HealthManager en escena");
            return;
        }

        if (debugMode) Debug.Log("⚠️ HealthManager no encontrado en escena, usando variable estática...");

        // OPCIÓN 3: Acceder directamente a la variable estática
        int saludAnterior = HealthManager.health;
        HealthManager.health -= damageAmount;
        HealthManager.health = Mathf.Max(0, HealthManager.health);

        if (debugMode)
        {
            Debug.Log("⚠️ Daño aplicado directamente a variable estática");
            Debug.Log("❤️ Salud anterior: " + saludAnterior);
            Debug.Log("❤️ Salud actual: " + HealthManager.health);
        }
    }

    private IEnumerator RespawnPlayerCoroutine(GameObject player)
    {
        isRespawning = true;

        if (debugMode) Debug.Log("📍 INICIANDO RESPAWN COROUTINE...");

        // ✅ 1. VERIFICAR VISIBILIDAD ANTES DEL RESPAWN
        if (forcePlayerVisibility)
        {
            CheckPlayerVisibility(player, "ANTES del respawn");
        }

        // ✅ 2. TELEPORT INMEDIATO
        Vector2 respawnPos = safeCheckpointPosition + respawnOffset;
        if (debugMode)
        {
            Debug.Log("📍 Posición antes del respawn: " + player.transform.position);
            Debug.Log("📍 Nueva posición de respawn: " + respawnPos);
        }

        player.transform.position = respawnPos;

        // ✅ 3. FORZAR CAPA CORRECTA
        if (forceSpecificLayer)
        {
            int layerAnterior = player.layer;
            player.layer = targetLayer;
            SetLayerRecursively(player.transform, targetLayer);

            if (debugMode)
            {
                Debug.Log("🏷️ Capa anterior: " + layerAnterior + " (" + LayerMask.LayerToName(layerAnterior) + ")");
                Debug.Log("🏷️ Capa nueva: " + targetLayer + " (" + LayerMask.LayerToName(targetLayer) + ")");
            }
        }

        // ✅ 4. RESETEAR FÍSICA
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 velocidadAnterior = rb.velocity;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if (debugMode)
            {
                Debug.Log("⚡ Física reseteada");
                Debug.Log("⚡ Velocidad anterior: " + velocidadAnterior);
            }
        }

        // ✅ 5. ESPERAR UN FRAME PARA QUE UNITY PROCESE LOS CAMBIOS
        yield return null;

        // ✅ 6. VERIFICAR VISIBILIDAD DESPUÉS DEL RESPAWN
        if (forcePlayerVisibility)
        {
            CheckPlayerVisibility(player, "DESPUÉS del respawn");
            ForcePlayerVisibility(player); // Forzar nuevamente por si acaso
        }

        // ✅ 7. EFECTO DE INMUNIDAD (OPCIONAL)
        if (enableImmunityEffect)
        {
            yield return StartCoroutine(ImmunityEffectCoroutine(player));
        }

        isRespawning = false;

        if (debugMode) Debug.Log("✅ RESPAWN COROUTINE COMPLETADA");
    }

    // ✅ MÉTODO PARA VERIFICAR VISIBILIDAD
    private void CheckPlayerVisibility(GameObject player, string momento)
    {
        if (debugMode) Debug.Log("👀 VERIFICANDO VISIBILIDAD - " + momento);

        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
        int problemasEncontrados = 0;

        foreach (Renderer renderer in renderers)
        {
            if (!renderer.enabled)
            {
                if (debugMode) Debug.LogWarning("❌ Renderer deshabilitado: " + renderer.name);
                problemasEncontrados++;
            }

            foreach (Material material in renderer.materials)
            {
                if (material.color.a < 0.99f)
                {
                    if (debugMode) Debug.LogWarning("🎨 Alpha bajo: " + material.color.a + " en " + renderer.name);
                    problemasEncontrados++;
                }
            }
        }

        if (debugMode)
        {
            Debug.Log("📊 Resumen de visibilidad - " + momento);
            Debug.Log("   └── Total renderers: " + renderers.Length);
            Debug.Log("   └── Problemas encontrados: " + problemasEncontrados);
        }
    }

    // ✅ EFECTO DE INMUNIDAD (PARPADEO)
    private IEnumerator ImmunityEffectCoroutine(GameObject player)
    {
        if (debugMode) Debug.Log("🛡️ Iniciando efecto de inmunidad");

        SpriteRenderer[] spriteRenderers = player.GetComponentsInChildren<SpriteRenderer>();
        float timer = 0f;
        bool isVisible = true;

        while (timer < immunityDuration && spriteRenderers.Length > 0)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = isVisible;
                }
            }

            isVisible = !isVisible;
            timer += 0.15f;
            yield return new WaitForSeconds(0.15f);
        }

        // ✅ GARANTIZAR VISIBILIDAD FINAL
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }

        if (debugMode) Debug.Log("🛡️ Efecto de inmunidad terminado");
    }

    private void SetLayerRecursively(Transform parent, int layer)
    {
        if (parent == null) return;

        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            if (child != null)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }

    public void UpdateSafeCheckpoint(Vector2 newSafePosition)
    {
        Vector2 posicionAnterior = safeCheckpointPosition;
        safeCheckpointPosition = newSafePosition;

        if (debugMode)
        {
            Debug.Log("🛡️ ACTUALIZANDO CHECKPOINT SEGURO:");
            Debug.Log("📍 Posición anterior: " + posicionAnterior);
            Debug.Log("📍 Nueva posición segura: " + safeCheckpointPosition);
        }
    }

    // ✅ MÉTODOS DE DEBUG PARA EL EDITOR
    [ContextMenu("🔍 Ver Estado del Hazard")]
    public void DebugHazardState()
    {
        Debug.Log("🔍 ===== ESTADO ACTUAL DEL HAZARD =====");
        Debug.Log("📍 Posición segura: " + safeCheckpointPosition);
        Debug.Log("💔 Daño: " + damageAmount);
        Debug.Log("❤️ Salud global: " + HealthManager.health);
        Debug.Log("🔄 Respawn en curso: " + isRespawning);
        Debug.Log("👀 Forzar visibilidad: " + forcePlayerVisibility);
        Debug.Log("🔍 =====================================");
    }

    [ContextMenu("👀 Verificar Visibilidad del Player")]
    public void CheckCurrentPlayerVisibility()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CheckPlayerVisibility(player, "MANUAL");
        }
        else
        {
            Debug.LogError("❌ No se encontró Player para verificar visibilidad");
        }
    }

    [ContextMenu("🔆 Forzar Visibilidad del Player AHORA")]
    public void ForceVisibilityNow()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ForcePlayerVisibility(player);
            Debug.Log("🔆 Visibilidad forzada manualmente");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar área del hazard
        Gizmos.color = Color.red;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }

        // Dibujar posición segura
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(safeCheckpointPosition + respawnOffset, new Vector3(1f, 1f, 0.1f));

        // Dibujar línea desde hazard hasta posición segura
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, safeCheckpointPosition + respawnOffset);
    }
}