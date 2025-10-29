using UnityEngine;

public class MiniJefe : MonoBehaviour
{
    [Header("CONFIGURACIÓN COMPORTAMIENTO")]
    public float velocidad = 2.5f;
    public float rangoAtaque = 1.5f;
    public float cooldownAtaque = 1.5f;
    public int dañoAtaque = 1;

    [Header("REFERENCIAS")]
    public PuertaFinal puertaFinal;

    [Header("DEBUG")]
    public bool debugMode = true;

    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float ultimoAtaque;
    private bool estaVivo = true;
    private bool estaVolteado = false;

    void Start()
    {
        InicializarReferencias();

        if (debugMode) Debug.Log("🎯 MiniJefe Comportamiento - Iniciado");
    }

    void InicializarReferencias()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (jugador == null && debugMode)
            Debug.LogWarning("⚠️ No se encontró jugador con tag 'Player'");
    }

    void Update()
    {
        if (!estaVivo) return;
        if (jugador == null)
        {
            BuscarJugador();
            return;
        }

        ControlarComportamiento();
    }

    void BuscarJugador()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
            if (debugMode) Debug.Log("🎯 Jugador encontrado");
        }
    }

    void ControlarComportamiento()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia > rangoAtaque)
        {
            PerseguirJugador();
        }
        else
        {
            DetenerMovimiento();
            if (Time.time - ultimoAtaque >= cooldownAtaque)
            {
                Atacar();
            }
        }

        VoltearHaciaJugador();
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.velocity = new Vector2(direccion.x * velocidad, rb.velocity.y);

        if (animator != null)
            animator.SetBool("Moviendose", true);

        if (debugMode && Time.frameCount % 60 == 0)
            Debug.Log("🚶 Persiguiendo jugador...");
    }

    void DetenerMovimiento()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (animator != null)
            animator.SetBool("Moviendose", false);
    }

    void VoltearHaciaJugador()
    {
        if (spriteRenderer == null) return;

        bool mirarDerecha = transform.position.x < jugador.position.x;

        if (mirarDerecha && estaVolteado)
        {
            spriteRenderer.flipX = false;
            estaVolteado = false;
        }
        else if (!mirarDerecha && !estaVolteado)
        {
            spriteRenderer.flipX = true;
            estaVolteado = true;
        }
    }

    void Atacar()
    {
        ultimoAtaque = Time.time;

        if (animator != null)
            animator.SetTrigger("Atacar");
        else
            AplicarDaño(); // Si no hay animator, aplicar daño inmediato

        if (debugMode) Debug.Log("⚔️ Atacando al jugador");
    }

    public void AplicarDaño()
    {
        if (EstaJugadorEnRango())
        {
            HealthManager healthManager = FindObjectOfType<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(dañoAtaque);
            }
            else
            {
                // Fallback
                HealthManager.health = Mathf.Max(0, HealthManager.health - dañoAtaque);
            }

            if (debugMode) Debug.Log($"💥 Daño aplicado: {dañoAtaque}");
        }
    }

    public void RecibirDaño(int daño = 1)
    {
        if (!estaVivo) return;

        if (debugMode) Debug.Log("🩸 Mini jefe recibió daño");
        DerrotarJefe();
    }

    public void DerrotarJefe()
    {
        if (!estaVivo) return;

        estaVivo = false;

        if (debugMode) Debug.Log("🎉 Mini jefe derrotado!");

        // Desactivar componentes
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Animación de muerte
        if (animator != null)
            animator.SetTrigger("Morir");
        else
            StartCoroutine(EfectoMuerteVisual());

        // Abrir puerta final
        if (puertaFinal != null)
        {
            Invoke("AbrirPuertaFinal", 1.5f);
        }

        if (debugMode) Debug.Log("🚪 Programando apertura de puerta final...");
    }

    System.Collections.IEnumerator EfectoMuerteVisual()
    {
        // Efecto visual simple si no hay animator
        if (spriteRenderer != null)
        {
            float duracion = 1f;
            float timer = 0f;
            Color colorInicial = spriteRenderer.color;

            while (timer < duracion)
            {
                timer += Time.deltaTime;
                float progreso = timer / duracion;

                // Fade out
                spriteRenderer.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1f - progreso);

                // Escala reducida
                transform.localScale = Vector3.one * (1f - progreso * 0.5f);

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }

    void AbrirPuertaFinal()
    {
        if (puertaFinal != null)
        {
            puertaFinal.AbrirPuerta();
            if (debugMode) Debug.Log("🎊 Puerta final abierta");
        }
    }

    bool EstaJugadorEnRango()
    {
        return jugador != null && Vector2.Distance(transform.position, jugador.position) <= rangoAtaque;
    }

    [ContextMenu("🔍 Ver Estado Comportamiento")]
    public void DebugEstado()
    {
        Debug.Log("🔍 ===== MINI JEFE COMPORTAMIENTO =====");
        Debug.Log("❤️ Vivo: " + estaVivo);
        Debug.Log("🎯 Jugador: " + (jugador != null));
        Debug.Log("⚡ Velocidad: " + velocidad);
        Debug.Log("📏 Rango Ataque: " + rangoAtaque);
        Debug.Log("🔍 =================================");
    }

    void OnDrawGizmosSelected()
    {
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque * 2f);
    }
}