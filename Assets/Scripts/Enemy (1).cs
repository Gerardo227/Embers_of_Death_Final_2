using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public float aggroRange = 15f; // ✅ AUMENTADO
    public float speed = 6f;       // ✅ AUMENTADO  
    public float jumpCooldown = 3f;
    public float jumpForce = 8f;

    float lastJump;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log($"🎯 Enemy INICIADO");
        Debug.Log($"📏 Aggro: {aggroRange}, Speed: {speed}");
        VerificarEstado(); // ✅ Verificar estado al inicio
    }

    void Update()
    {
        if (player == null)
        {
            Debug.Log("❌ Player es NULL");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        // ✅ DEBUG CADA SEGUNDO
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"📐 Distancia: {distance:F1} / Aggro: {aggroRange}");
        }

        if (distance < aggroRange)
        {
            Debug.Log($"🎯 Player EN RANGO - Moviendo...");

            // Movimiento en X
            if (player.transform.position.x < transform.position.x)
            {
                // Player a la IZQUIERDA
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                if (spriteRenderer != null) spriteRenderer.flipX = true;
                Debug.Log($"👈 Moviendo IZQUIERDA - Speed: {speed}");
            }
            else
            {
                // Player a la DERECHA  
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                if (spriteRenderer != null) spriteRenderer.flipX = false;
                Debug.Log($"👉 Moviendo DERECHA - Speed: {speed}");
            }

            // Salto
            if (Time.time > lastJump + jumpCooldown && rb != null)
            {
                lastJump = Time.time;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                Debug.Log("🦘 Saltando!");
            }
        }
        else
        {
            if (Time.frameCount % 120 == 0) // ✅ Debug cada 2 segundos cuando está fuera de rango
            {
                Debug.Log($"😴 Player FUERA de rango: {distance:F1} > {aggroRange}");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("💥 Colisión con Player!");
            HealthManager health = collision.gameObject.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(1);
                Debug.Log("💥 Daño aplicado");
            }
        }
    }

    [ContextMenu("🔍 Verificar Estado")]
    public void VerificarEstado()
    {
        Debug.Log("=== ESTADO ENEMY ===");
        Debug.Log($"📍 Enemy posición: {transform.position}");
        Debug.Log($"🎯 Player posición: {player?.transform.position}");
        Debug.Log($"📏 Aggro Range: {aggroRange}");
        Debug.Log($"🚀 Speed: {speed}");

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"📐 Distancia real: {dist}");
            Debug.Log($"👁️ Player a la: {(player.transform.position.x < transform.position.x ? "IZQUIERDA" : "DERECHA")}");
            Debug.Log($"🎯 En rango: {dist < aggroRange} ({dist} < {aggroRange})");
        }
    }

    [ContextMenu("⚡ Aumentar Aggro a 20")]
    public void Aggro20()
    {
        aggroRange = 20f;
        Debug.Log($"🔧 Aggro aumentado a: {aggroRange}");
        VerificarEstado();
    }

    [ContextMenu("⚡ Aumentar Speed a 8")]
    public void Speed8()
    {
        speed = 8f;
        Debug.Log($"🔧 Speed aumentado a: {speed}");
    }

    [ContextMenu("⚡ Valores Extremos: Aggro 30, Speed 10")]
    public void ValoresExtremos()
    {
        aggroRange = 30f;
        speed = 10f;
        Debug.Log($"🔧 VALORES EXTREMOS - Aggro: {aggroRange}, Speed: {speed}");
        VerificarEstado();
    }
}