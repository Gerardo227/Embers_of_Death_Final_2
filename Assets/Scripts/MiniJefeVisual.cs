using UnityEngine;
using System.Collections;

public class MiniJefeVisual : MonoBehaviour
{
    [Header("CONFIGURACIÓN VISUAL")]
    public Sprite spriteJefe;
    public Color colorJefe = Color.white;
    public float escala = 1.5f;
    public string sortingLayer = "Characters";
    public int orderInLayer = 5;

    [Header("CONFIGURACIÓN SPAWNEO")]
    public float delaySpawneo = 1f;
    public bool spawnAlActivar = true;
    public bool usarAnimacionEntrada = true;
    public float duracionAnimacion = 1f;

    [Header("COMPONENTES")]
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D colliderJefe;
    private MiniJefe comportamientoJefe;

    [Header("DEBUG")]
    public bool debugMode = true;
    public KeyCode teclaDebugRespawn = KeyCode.R;

    void Start()
    {
        InicializarComponentes();
        ConfigurarEstadoInicial();

        if (spawnAlActivar)
        {
            StartCoroutine(SpawneoVisual());
        }

        if (debugMode) Debug.Log("🎬 MiniJefeVisual - Script iniciado");
    }

    void InicializarComponentes()
    {
        // Obtener o agregar componentes necesarios
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        colliderJefe = GetComponent<Collider2D>();
        if (colliderJefe == null)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(0.8f, 0.8f);
            colliderJefe = boxCollider;
        }

        comportamientoJefe = GetComponent<MiniJefe>();

        if (debugMode) Debug.Log("🔧 Componentes inicializados");
    }

    void ConfigurarEstadoInicial()
    {
        // Configurar sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Invisible al inicio

            if (spriteJefe != null)
            {
                spriteRenderer.sprite = spriteJefe;
                if (debugMode) Debug.Log("🎨 Sprite asignado: " + spriteJefe.name);
            }
            else
            {
                Debug.LogWarning("⚠️ No hay sprite asignado para el mini jefe");
            }

            spriteRenderer.color = colorJefe;
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sortingOrder = orderInLayer;
        }

        // Configurar física
        if (rb != null)
        {
            rb.gravityScale = 3f;
            rb.freezeRotation = true;
            rb.simulated = false; // Desactivar física hasta el spawneo
        }

        // Configurar collider
        if (colliderJefe != null)
        {
            colliderJefe.enabled = false;
        }

        // Configurar escala
        transform.localScale = Vector3.one * escala;

        if (debugMode) Debug.Log("⚙️ Estado inicial configurado - INVISIBLE");
    }

    IEnumerator SpawneoVisual()
    {
        if (debugMode) Debug.Log("🎯 Iniciando secuencia de spawneo...");

        // Esperar delay inicial
        yield return new WaitForSeconds(delaySpawneo);

        // Activar sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            if (debugMode) Debug.Log("👁️ Sprite activado");
        }

        // Animación de entrada
        if (usarAnimacionEntrada && spriteRenderer != null)
        {
            yield return StartCoroutine(AnimacionEntrada());
        }

        // Activar componentes de gameplay
        ActivarGameplay();

        if (debugMode) Debug.Log("✅ Mini jefe completamente activado y visible");
    }

    IEnumerator AnimacionEntrada()
    {
        if (debugMode) Debug.Log("🎬 Iniciando animación de entrada");

        float timer = 0f;
        Color colorInicial = new Color(colorJefe.r, colorJefe.g, colorJefe.b, 0f);
        Color colorFinal = colorJefe;

        Vector3 escalaInicial = Vector3.one * 0.1f;
        Vector3 escalaFinal = Vector3.one * escala;

        while (timer < duracionAnimacion)
        {
            timer += Time.deltaTime;
            float progreso = timer / duracionAnimacion;
            float curva = Mathf.Sin(progreso * Mathf.PI * 0.5f); // Curva suave

            // Fade in
            spriteRenderer.color = Color.Lerp(colorInicial, colorFinal, curva);

            // Escala progresiva
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, curva);

            yield return null;
        }

        // Asegurar estado final
        spriteRenderer.color = colorFinal;
        transform.localScale = escalaFinal;

        if (debugMode) Debug.Log("✨ Animación de entrada completada");
    }

    void ActivarGameplay()
    {
        // Activar física
        if (rb != null)
        {
            rb.simulated = true;
        }

        // Activar collider
        if (colliderJefe != null)
        {
            colliderJefe.enabled = true;
        }

        // Activar comportamiento del jefe
        if (comportamientoJefe != null)
        {
            comportamientoJefe.enabled = true;
        }

        if (debugMode) Debug.Log("🎮 Componentes de gameplay activados");
    }

    void Update()
    {
        // Debug por teclado
        if (Input.GetKeyDown(teclaDebugRespawn))
        {
            Respawnear();
        }
    }

    [ContextMenu("🔄 Respawnear Jefe")]
    public void Respawnear()
    {
        if (debugMode) Debug.Log("🔄 Respawneando jefe...");

        StopAllCoroutines();
        ConfigurarEstadoInicial();
        StartCoroutine(SpawneoVisual());
    }

    [ContextMenu("👁️ Hacer Visible Inmediato")]
    public void HacerVisibleInmediato()
    {
        StopAllCoroutines();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = colorJefe;
        }

        transform.localScale = Vector3.one * escala;
        ActivarGameplay();

        if (debugMode) Debug.Log("👁️ Jefe visible inmediatamente");
    }

    void OnBecameVisible()
    {
        if (debugMode) Debug.Log("📷 Mini jefe entró en la vista de cámara");
    }

    void OnBecameInvisible()
    {
        if (debugMode) Debug.Log("📷 Mini jefe salió de la vista de cámara");
    }

    void OnDrawGizmos()
    {
        // Dibujar área del jefe
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0) * escala);

        // Icono identificador
        Gizmos.color = Color.yellow;
        Gizmos.DrawIcon(transform.position + Vector3.up * 0.7f, "enemyicon", true);

        // Texto debug
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f, "🪳 MINI JEFE");
#endif
    }
}