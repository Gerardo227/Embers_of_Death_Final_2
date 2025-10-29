using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static int health = 3;
    public int maxHealth = 3;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Start()
    {
        Debug.Log("🔄 HealthManager.Start() iniciado");
        health = maxHealth;

        // Verificar UI
        if (hearts == null)
        {
            Debug.LogError("❌ HealthManager: Array 'hearts' es NULL");
        }
        else if (hearts.Length == 0)
        {
            Debug.LogError("❌ HealthManager: Array 'hearts' está VACÍO");
        }
        else
        {
            Debug.Log($"✅ HealthManager: {hearts.Length} corazones configurados");
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] == null)
                {
                    Debug.LogError($"❌ HealthManager: Corazón en índice {i} es NULL");
                }
            }
        }

        UpdateHearts();
        Debug.Log($"❤️ HealthManager: Salud inicializada - {health}/{maxHealth}");
    }

    void Update()
    {
        UpdateHearts();

        if (health <= 0 && !PlayerManager.isGameOver)
        {
            Debug.Log("💀 HealthManager: Salud llegó a 0 - llamando Die()");
            Die();
        }
    }

    public void UpdateHearts()
    {
        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogWarning("⚠️ HealthManager: No se puede actualizar UI - hearts no configurado");
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].sprite = i < health ? fullHeart : emptyHeart;
            }
        }

        Debug.Log($"🔄 HealthManager: UI actualizado - Salud: {health}/{maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"💥 HealthManager.TakeDamage() llamado - Daño: {damage}, Salud antes: {health}");

        health -= damage;
        health = Mathf.Max(0, health);

        Debug.Log($"❤️ HealthManager: Salud después: {health}");

        UpdateHearts();

        if (health <= 0)
        {
            Debug.Log("💀 HealthManager: Salud <= 0 - llamando Die()");
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 HealthManager.Die() iniciado");
        PlayerManager.isGameOver = true;

        // Desactivar movimiento
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
            Debug.Log("✅ HealthManager: PlayerMovement desactivado");
        }

        // Detener física
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            Debug.Log("✅ HealthManager: Física detenida");
        }

        Debug.Log("🎮 HealthManager: Jugador completamente desactivado");
    }

    // ✅ MÉTODO PÚBLICO PARA CURAR
    public void Heal(int cantidad)
    {
        int saludAnterior = health;
        health = Mathf.Min(health + cantidad, maxHealth);
        UpdateHearts();
        Debug.Log($"💚 HealthManager: Curado +{cantidad}. Salud: {saludAnterior} → {health}");
    }

    // ✅ MÉTODO PARA AÑADIR CORAZÓN PERMANENTE
    public void AddPermanentHeart()
    {
        // Aumentar la salud máxima permanentemente
        maxHealth++;
        // Llenar el nuevo corazón
        health = maxHealth;

        Debug.Log($"💖 HealthManager: Corazón PERMANENTE añadido. Salud máxima: {maxHealth}");
        UpdateHearts();
    }

    // ✅ MÉTODO PARA AUMENTAR MÁXIMA SALUD SIN CURAR
    public void IncreaseMaxHealth(int cantidad = 1)
    {
        maxHealth += cantidad;
        Debug.Log($"💖 HealthManager: Salud máxima aumentada +{cantidad}. Nueva salud máxima: {maxHealth}");
        UpdateHearts();
    }

    // ✅ MÉTODO PARA AUMENTAR SALUD Y MÁXIMA SALUD
    public void AddHealthAndMaxHealth(int cantidad)
    {
        int saludAnterior = health;
        int maxSaludAnterior = maxHealth;

        maxHealth += cantidad;
        health = maxHealth;

        Debug.Log($"💖 HealthManager: Salud y máxima salud aumentadas +{cantidad}. " +
                 $"Salud: {saludAnterior}→{health}, Máxima: {maxSaludAnterior}→{maxHealth}");
        UpdateHearts();
    }

    // ✅ MÉTODO PARA RESTABLECER SALUD AL MÁXIMO
    public void RestoreToMaxHealth()
    {
        int saludAnterior = health;
        health = maxHealth;
        UpdateHearts();
        Debug.Log($"💚 HealthManager: Salud restaurada al máximo. Salud: {saludAnterior} → {health}");
    }

    // ✅ MÉTODO PARA VERIFICAR ESTADO ACTUAL
    public void PrintCurrentStatus()
    {
        Debug.Log($"📊 HealthManager Status: Salud: {health}/{maxHealth}, Corazones UI: {hearts.Length}");
    }
}