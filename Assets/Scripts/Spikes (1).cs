using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Configuración de Spikes")]
    public bool debugMode = true;
    public int damageAmount = 1;
    public bool allowJumping = true;

    [Header("Configuración de Daño")]
    public float damageCooldown = 2f;
    public bool forcePlayerVisibility = true;
    public float knockbackForce = 8f;

    private float lastDamageTime;
    private GameObject lastDamagedPlayer;
    private bool playerIsOnTop = false;

    private void Start()
    {
        if (debugMode) Debug.Log("✅ Spikes inicializados - Saltos permitidos");
    }

    // ✅ MÉTODO PARA COLISIÓN NORMAL
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (debugMode) Debug.Log("🎯 Colisión ENTER con: " + collision.gameObject.name);

        if (IsPlayer(collision.gameObject))
        {
            // Verificar si el player está saltando sobre los spikes
            if (allowJumping && IsPlayerOnTop(collision))
            {
                playerIsOnTop = true;
                if (debugMode) Debug.Log("🦘 Player saltando sobre spikes - Sin daño");
                return; // Permitir saltar sin daño
            }

            HandlePlayerCollision(collision.gameObject, collision);
        }
    }

    // ✅ MÉTODO PARA CUANDO EL PLAYER SE MANTIENE EN CONTACTO
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            // Verificar si el player está encima (saltando)
            if (allowJumping && IsPlayerOnTop(collision))
            {
                playerIsOnTop = true;
                if (debugMode) Debug.Log("🦘 Player manteniendo salto sobre spikes");
                return;
            }

            // Si no está encima y no estamos en cooldown, aplicar daño
            if (!playerIsOnTop && Time.time - lastDamageTime >= damageCooldown)
            {
                HandlePlayerCollision(collision.gameObject, collision);
            }
        }
    }

    // ✅ MÉTODO PARA CUANDO EL PLAYER SALE DEL CONTACTO
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            playerIsOnTop = false;
            if (debugMode) Debug.Log("🚶 Player salió de los spikes");
        }
    }

    // ✅ MÉTODO PARA TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (debugMode) Debug.Log("🎯 Trigger ENTER con: " + collision.gameObject.name);

        if (IsPlayer(collision.gameObject))
        {
            // Para trigger, siempre aplicar daño (spikes laterales)
            HandlePlayerCollision(collision.gameObject, null);
        }
    }

    // ✅ VERIFICAR SI EL PLAYER ESTÁ ENCIMA (SALTANDO)
    private bool IsPlayerOnTop(Collision2D collision)
    {
        if (collision.contacts.Length == 0) return false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Si la normal apunta hacia arriba (player encima)
            if (contact.normal.y > 0.7f) // Aumentado el threshold para ser más permisivo
            {
                if (debugMode) Debug.Log("📐 Player ENCIMA - Normal: " + contact.normal + " - Sin daño");
                return true;
            }
        }
        return false;
    }

    // ✅ MANEJAR COLISIÓN CON PLAYER
    private void HandlePlayerCollision(GameObject player, Collision2D collision = null)
    {
        // Si allowJumping está activado y el player está encima, no hacer daño
        if (allowJumping && playerIsOnTop)
        {
            if (debugMode) Debug.Log("🦘 Saltando - No aplicar daño");
            return;
        }

        // Verificar cooldown de daño
        if (Time.time - lastDamageTime < damageCooldown)
        {
            if (debugMode) Debug.Log("⏰ En cooldown, sin daño");
            return;
        }

        if (debugMode) Debug.Log("💀 Player tocó spikes - Aplicando SOLO 1 DAÑO");

        // Forzar visibilidad si está activado
        if (forcePlayerVisibility)
        {
            ForcePlayerVisibility(player);
        }

        // ✅ SOLO APLICAR DAÑO UNA VEZ
        ApplyDamageToPlayer(player);

        // ✅ EMPUJÓN FUERTE PARA SACAR AL PLAYER
        ApplyKnockback(player, collision);

        // Actualizar tiempos
        lastDamageTime = Time.time;
        lastDamagedPlayer = player;

        if (debugMode) Debug.Log("✅ Daño aplicado - Cooldown activado");
    }

    // ✅ APLICAR SOLO 1 DE DAÑO
    private void ApplyDamageToPlayer(GameObject player)
    {
        if (debugMode) Debug.Log("❤️ Aplicando SOLO 1 de daño");

        int saludAnterior = HealthManager.health;

        // Buscar HealthManager en el player
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.TakeDamage(damageAmount);
            if (debugMode)
            {
                Debug.Log("✅ 1 de daño aplicado");
                Debug.Log("❤️ Salud anterior: " + saludAnterior);
                Debug.Log("❤️ Salud actual: " + HealthManager.health);
            }
            return;
        }

        // Buscar HealthManager en la escena
        HealthManager healthManagerInScene = FindObjectOfType<HealthManager>();
        if (healthManagerInScene != null)
        {
            healthManagerInScene.TakeDamage(damageAmount);
            if (debugMode)
            {
                Debug.Log("✅ 1 de daño aplicado en escena");
                Debug.Log("❤️ Salud anterior: " + saludAnterior);
                Debug.Log("❤️ Salud actual: " + HealthManager.health);
            }
            return;
        }

        // Usar variable estática (solo 1 de daño)
        HealthManager.health = Mathf.Max(0, HealthManager.health - damageAmount);

        if (debugMode)
        {
            Debug.Log("⚠️ 1 de daño aplicado directamente");
            Debug.Log("❤️ Salud anterior: " + saludAnterior);
            Debug.Log("❤️ Salud actual: " + HealthManager.health);
        }
    }

    // ✅ EMPUJÓN FUERTE PERO INTELIGENTE
    private void ApplyKnockback(GameObject player, Collision2D collision = null)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackDirection = Vector2.zero;

            if (collision != null && collision.contacts.Length > 0)
            {
                // Empujar en dirección opuesta al punto de contacto
                Vector2 contactNormal = collision.contacts[0].normal;
                knockbackDirection = -contactNormal;

                // Si el player está mayormente horizontal, empujar más hacia arriba
                if (Mathf.Abs(knockbackDirection.x) > Mathf.Abs(knockbackDirection.y))
                {
                    knockbackDirection.y = 0.7f; // Más componente vertical
                    knockbackDirection = knockbackDirection.normalized;
                }

                if (debugMode) Debug.Log("📐 Empujón direccional: " + knockbackDirection);
            }
            else
            {
                // Empujón por defecto (hacia arriba y un poco aleatorio)
                float randomX = Random.Range(-0.3f, 0.3f);
                knockbackDirection = new Vector2(randomX, 0.8f).normalized;

                if (debugMode) Debug.Log("📐 Empujón por defecto: " + knockbackDirection);
            }

            // Aplicar empujón
            rb.velocity = Vector2.zero; // Resetear velocidad primero
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            if (debugMode) Debug.Log("⚡ Empujón aplicado: " + (knockbackDirection * knockbackForce));
        }
    }

    // ✅ VERIFICAR SI ES PLAYER
    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.GetComponent<PlayerManager>() != null;
    }

    // ✅ FORZAR VISIBILIDAD
    private void ForcePlayerVisibility(GameObject player)
    {
        if (!forcePlayerVisibility) return;

        Renderer[] allRenderers = player.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in allRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
                foreach (Material material in renderer.materials)
                {
                    if (material != null)
                    {
                        Color currentColor = material.color;
                        material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                    }
                }
            }
        }
    }

    // ✅ MÉTODOS DE DEBUG
    [ContextMenu("🔍 Ver Estado de Spikes")]
    public void DebugSpikesState()
    {
        Debug.Log("🔍 ===== SPIKES (CON SALTOS) =====");
        Debug.Log("💔 Daño por toque: " + damageAmount);
        Debug.Log("🦘 Saltos permitidos: " + allowJumping);
        Debug.Log("⏰ Cooldown: " + damageCooldown + "s");
        Debug.Log("👤 Player encima: " + playerIsOnTop);
        Debug.Log("❤️ Salud global: " + HealthManager.health);
        Debug.Log("🔍 ===============================");
    }

    [ContextMenu("🔄 Reiniciar Cooldown")]
    public void ResetCooldown()
    {
        lastDamageTime = 0f;
        lastDamagedPlayer = null;
        playerIsOnTop = false;
        Debug.Log("🔄 Cooldown y estado reiniciados");
    }

    // ✅ DIBUJAR GIZMOS PARA VISUALIZAR
    private void OnDrawGizmosSelected()
    {
        // Dibujar área de spikes
        Gizmos.color = Color.red;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
        }

        // Indicador visual de permitir saltos
        if (allowJumping)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1f, "Animation.NextKey", true);

            // Mostrar área de salto segura
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            if (collider is BoxCollider2D box)
            {
                Vector3 safeArea = new Vector3(box.size.x * 0.8f, 0.1f, 0.1f);
                Gizmos.DrawCube(transform.position + Vector3.up * (box.size.y / 2 + 0.05f), safeArea);
            }
        }
    }
}