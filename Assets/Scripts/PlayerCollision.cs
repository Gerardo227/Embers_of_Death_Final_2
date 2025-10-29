using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private HealthManager healthManager;
    private bool canTakeDamage = true;
    private float damageCooldown = 1f;

    void Start()
    {
        Debug.Log("🔄 PlayerCollision.Start() iniciado");
        healthManager = GetComponent<HealthManager>();

        if (healthManager == null)
        {
            Debug.LogError("❌ PlayerCollision: HealthManager no encontrado!");
        }
        else
        {
            Debug.Log("✅ PlayerCollision: HealthManager encontrado");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🔵 PlayerCollision.OnCollisionEnter2D con: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"⚡ PlayerCollision: Enemigo detectado - canTakeDamage: {canTakeDamage}");

            if (canTakeDamage)
            {
                Debug.Log("⚡ PlayerCollision: Aplicando daño desde colisión entrante");
                ApplyDamage();
            }
            else
            {
                Debug.Log("🛡️ PlayerCollision: Daño ignorado - en período de inmunidad (OnCollisionEnter2D)");
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"⚡ PlayerCollision: Colisión continua con enemigo - canTakeDamage: {canTakeDamage}");

            // ✅ CORRECCIÓN: Verificar canTakeDamage ANTES de aplicar daño
            if (canTakeDamage)
            {
                Debug.Log("⚡ PlayerCollision: Aplicando daño desde colisión continua");
                ApplyDamage();
            }
            else
            {
                Debug.Log("🛡️ PlayerCollision: Daño ignorado - en período de inmunidad (OnCollisionStay2D)");
            }
        }
    }

    void ApplyDamage()
    {
        Debug.Log($"🛡️ PlayerCollision.ApplyDamage - canTakeDamage: {canTakeDamage}");

        // ✅ CORRECCIÓN: Verificación adicional de seguridad
        if (!canTakeDamage)
        {
            Debug.Log("⏳ PlayerCollision: Cooldown activo - ignorando daño en ApplyDamage()");
            return;
        }

        if (healthManager != null)
        {
            // ✅ CORRECCIÓN: Desactivar canTakeDamage INMEDIATAMENTE antes de aplicar daño
            canTakeDamage = false;
            Debug.Log("🛡️ PlayerCollision: Cooldown activado INMEDIATAMENTE");

            healthManager.TakeDamage(1);
            StartCoroutine(GetHurt());

            // ✅ CORRECCIÓN: Usar Invoke para resetear el cooldown
            Invoke(nameof(ResetDamageCooldown), damageCooldown);
            Debug.Log($"⏰ PlayerCollision: Cooldown programado por {damageCooldown} segundos");
        }
        else
        {
            Debug.LogError("❌ PlayerCollision: HealthManager es NULL - no se puede aplicar daño");
        }
    }

    void ResetDamageCooldown()
    {
        canTakeDamage = true;
        Debug.Log("🛡️ PlayerCollision: Cooldown terminado - listo para recibir daño");
    }

    IEnumerator GetHurt()
    {
        Debug.Log("🎭 PlayerCollision: Iniciando animación de daño");

        // Ignorar colisión con enemigos temporalmente
        Physics2D.IgnoreLayerCollision(6, 8, true);
        Debug.Log("🛡️ PlayerCollision: Colisiones con enemigos ignoradas");

        // Animación de daño
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetLayerWeight(1, 1);
            Debug.Log("✅ PlayerCollision: Animación de daño activada");
        }

        Debug.Log("⏰ PlayerCollision: Esperando 2 segundos...");
        yield return new WaitForSeconds(2);

        // Restaurar
        if (animator != null)
        {
            animator.SetLayerWeight(1, 0);
            Debug.Log("✅ PlayerCollision: Animación de daño desactivada");
        }

        Physics2D.IgnoreLayerCollision(6, 8, false);
        Debug.Log("🛡️ PlayerCollision: Colisiones con enemigos restauradas");
        Debug.Log("🎭 PlayerCollision: Animación de daño terminada");
    }

    // ✅ NUEVO: Método para debug manual del estado de daño
    [ContextMenu("Verificar Estado de Daño")]
    public void CheckDamageState()
    {
        Debug.Log($"🔍 PlayerCollision: Estado actual - canTakeDamage: {canTakeDamage}");
        Debug.Log($"🔍 PlayerCollision: HealthManager: {healthManager != null}");

        if (healthManager != null)
        {
            Debug.Log($"🔍 PlayerCollision: Salud actual: {HealthManager.health}");
        }
    }

    // ✅ NUEVO: Método para forzar reset del cooldown
    [ContextMenu("Forzar Reset Cooldown")]
    public void ForceResetCooldown()
    {
        canTakeDamage = true;
        Debug.Log("🔄 PlayerCollision: Cooldown forzado manualmente");
    }
}