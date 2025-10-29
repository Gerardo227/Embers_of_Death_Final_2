using UnityEngine;

public class PuertaFinal : MonoBehaviour
{
    [Header("CONFIGURACIÓN PUERTA")]
    public string mensajeFinal = "¡DEMO COMPLETADO!";

    [Header("COMPONENTES VISUALES")]
    public GameObject visualCerrado;
    public GameObject visualAbierto;

    [Header("COMPONENTES FÍSICOS")]
    public Collider2D colliderBloqueo;

    [Header("UI FINAL")]
    public GameObject panelFinal;
    public UnityEngine.UI.Text textoFinal;

    [Header("DEBUG")]
    public bool debugMode = true;

    private bool estaAbierta = false;

    void Start()
    {
        ConfigurarEstadoInicial();

        if (debugMode)
            Debug.Log("🚪 PuertaFinal - Inicializada (CERRADA)");
    }

    void ConfigurarEstadoInicial()
    {
        estaAbierta = false;

        // Configurar visuales
        if (visualCerrado != null)
            visualCerrado.SetActive(true);
        if (visualAbierto != null)
            visualAbierto.SetActive(false);

        // Configurar física
        if (colliderBloqueo != null)
            colliderBloqueo.enabled = true;

        // Configurar UI
        if (panelFinal != null)
            panelFinal.SetActive(false);
        if (textoFinal != null)
        {
            textoFinal.text = mensajeFinal;
            textoFinal.gameObject.SetActive(false);
        }
    }

    public void AbrirPuerta()
    {
        if (estaAbierta)
        {
            if (debugMode) Debug.Log("⚠️ Puerta ya estaba abierta");
            return;
        }

        estaAbierta = true;

        if (debugMode)
            Debug.Log($"🎊 Abriendo puerta final: {mensajeFinal}");

        // Cambiar apariencia
        if (visualCerrado != null)
            visualCerrado.SetActive(false);
        if (visualAbierto != null)
            visualAbierto.SetActive(true);

        // Remover bloqueo
        if (colliderBloqueo != null)
            colliderBloqueo.enabled = false;

        // Mostrar UI
        MostrarMensajeFinal();
    }

    void MostrarMensajeFinal()
    {
        if (panelFinal != null)
        {
            panelFinal.SetActive(true);
            if (debugMode) Debug.Log("📱 Panel final activado");
        }

        if (textoFinal != null)
        {
            textoFinal.gameObject.SetActive(true);
            if (debugMode) Debug.Log("📝 Texto final mostrado");
        }

        // Mensaje en consola como backup
        Debug.Log($"🎮 {mensajeFinal}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && estaAbierta)
        {
            if (debugMode)
                Debug.Log("🎮 Jugador pasó por la puerta final del demo");

            // Aquí puedes agregar más lógica:
            // - Cargar siguiente escena
            // - Mostrar créditos
            // - Guardar progreso, etc.
        }
    }

    [ContextMenu("🚪 Abrir Puerta Manualmente")]
    public void AbrirPuertaManual()
    {
        AbrirPuerta();
    }

    [ContextMenu("🔒 Cerrar Puerta Manualmente")]
    public void CerrarPuertaManual()
    {
        estaAbierta = false;

        if (visualCerrado != null) visualCerrado.SetActive(true);
        if (visualAbierto != null) visualAbierto.SetActive(false);
        if (colliderBloqueo != null) colliderBloqueo.enabled = true;
        if (panelFinal != null) panelFinal.SetActive(false);
        if (textoFinal != null) textoFinal.gameObject.SetActive(false);

        if (debugMode) Debug.Log("🔒 Puerta cerrada manualmente");
    }

    void OnDrawGizmos()
    {
        if (estaAbierta)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(1.2f, 2.2f, 0.1f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(1.2f, 2.2f, 0.1f));
        }
    }
}