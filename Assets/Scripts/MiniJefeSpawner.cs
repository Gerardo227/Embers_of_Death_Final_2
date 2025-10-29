using UnityEngine;
using System.Collections;

public class MiniJefeSpawner : MonoBehaviour
{
    [Header("Configuración de Spawneo")]
    public GameObject prefabMiniJefe;
    public Transform spawnPoint;
    public float delayAparecer = 1f;
    public bool spawnAlIniciar = false;

    [Header("Efectos Visuales")]
    public ParticleSystem efectoAparicion;
    public AudioClip sonidoAparicion;
    public Light luzAparicion;

    [Header("Animación de Entrada")]
    public bool usarAnimacionEntrada = true;
    public float escalaInicial = 0.1f;
    public float duracionAnimacion = 1.5f;

    [Header("Debug")]
    public bool debugMode = true;

    private GameObject miniJefeInstance;
    private bool yaSpawnedo = false;

    void Start()
    {
        if (spawnAlIniciar)
        {
            Invoke("SpawnearMiniJefe", delayAparecer);
        }

        if (debugMode) Debug.Log("✅ MiniJefeSpawner inicializado");
    }

    public void SpawnearMiniJefe()
    {
        if (yaSpawnedo) return;

        if (debugMode) Debug.Log("🎯 Iniciando spawneo del mini jefe...");

        StartCoroutine(SpawnearConEfectos());
    }

    IEnumerator SpawnearConEfectos()
    {
        yaSpawnedo = true;

        // 1. Efectos previos a la aparición
        if (efectoAparicion != null)
        {
            efectoAparicion.Play();
            if (debugMode) Debug.Log("✨ Efecto de partículas activado");
        }

        if (luzAparicion != null)
        {
            luzAparicion.enabled = true;
            if (debugMode) Debug.Log("💡 Luz de aparición activada");
        }

        // 2. Sonido de aparición
        if (sonidoAparicion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoAparicion, spawnPoint.position);
            if (debugMode) Debug.Log("🔊 Sonido de aparición reproducido");
        }

        // 3. Esperar un momento para drama
        yield return new WaitForSeconds(0.5f);

        // 4. Instanciar el mini jefe
        if (prefabMiniJefe != null && spawnPoint != null)
        {
            miniJefeInstance = Instantiate(prefabMiniJefe, spawnPoint.position, Quaternion.identity);

            // Configurar estado inicial (invisible o pequeño)
            if (usarAnimacionEntrada)
            {
                StartCoroutine(AnimacionEntrada(miniJefeInstance));
            }

            if (debugMode) Debug.Log("🪳 Mini jefe instanciado en escena");
        }
        else
        {
            Debug.LogError("❌ No hay prefab o spawn point configurado");
        }

        // 5. Apagar efectos después de un tiempo
        yield return new WaitForSeconds(1f);

        if (luzAparicion != null)
        {
            luzAparicion.enabled = false;
        }
    }

    IEnumerator AnimacionEntrada(GameObject jefe)
    {
        if (jefe == null) yield break;

        // Guardar escala normal
        Vector3 escalaNormal = jefe.transform.localScale;

        // Escala inicial muy pequeña
        jefe.transform.localScale = new Vector3(escalaInicial, escalaInicial, escalaInicial);

        // Hacer invisible temporalmente (opcional)
        SpriteRenderer sprite = jefe.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color colorOriginal = sprite.color;
            sprite.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
        }

        if (debugMode) Debug.Log("🎬 Iniciando animación de entrada");

        // Animación de crecimiento y aparición
        float timer = 0f;
        while (timer < duracionAnimacion)
        {
            timer += Time.deltaTime;
            float progreso = timer / duracionAnimacion;

            // Escala progresiva
            float escalaActual = Mathf.Lerp(escalaInicial, 1f, progreso);
            jefe.transform.localScale = new Vector3(escalaActual, escalaActual, escalaActual) * escalaNormal.x;

            // Alpha progresivo
            if (sprite != null)
            {
                float alpha = Mathf.Lerp(0.3f, 1f, progreso);
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            }

            yield return null;
        }

        // Asegurar escala y alpha final
        jefe.transform.localScale = escalaNormal;
        if (sprite != null)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }

        if (debugMode) Debug.Log("✅ Animación de entrada completada");
    }

    [ContextMenu("🎯 Spawnear Jefe Ahora")]
    public void SpawnearAhora()
    {
        SpawnearMiniJefe();
    }

    [ContextMenu("🗑️ Eliminar Jefe Actual")]
    public void EliminarJefe()
    {
        if (miniJefeInstance != null)
        {
            Destroy(miniJefeInstance);
            yaSpawnedo = false;
            if (debugMode) Debug.Log("🗑️ Jefe actual eliminado");
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar spawn point
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            Gizmos.DrawIcon(spawnPoint.position + Vector3.up * 0.7f, "spawnicon", true);
        }
    }
}