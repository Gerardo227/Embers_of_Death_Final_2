using UnityEngine;
using UnityEngine.UI;

public class BotonDebug : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public Button botonMataJefe;
    public Text textoBoton;

    [Header("DEBUG")]
    public bool debugMode = true;

    void Start()
    {
        if (botonMataJefe != null)
        {
            botonMataJefe.onClick.AddListener(MataJefeYAbrePuerta);
        }

        if (debugMode) Debug.Log("✅ Botón Debug - Inicializado");
    }

    public void MataJefeYAbrePuerta()
    {
        if (debugMode) Debug.Log("🎯 Botón Debug: Ejecutando...");

        // Buscar y matar mini jefe
        MiniJefe jefe = FindObjectOfType<MiniJefe>();
        if (jefe != null && jefe.enabled)
        {
            jefe.DerrotarJefe();
            if (debugMode) Debug.Log("🎯 Jefe eliminado");
        }

        // Buscar y abrir puerta final
        PuertaFinal puerta = FindObjectOfType<PuertaFinal>();
        if (puerta != null)
        {
            puerta.AbrirPuerta();
            if (debugMode) Debug.Log("🚪 Puerta abierta");
        }

        // Dar recompensas
        HealthManager.health = 3;
        HealthManager healthManager = FindObjectOfType<HealthManager>();
        if (healthManager != null) healthManager.UpdateHearts();

        // Feedback visual del botón
        if (textoBoton != null)
        {
            textoBoton.text = "¡HECHO!";
            Invoke("RestaurarTextoBoton", 2f);
        }
    }

    void RestaurarTextoBoton()
    {
        if (textoBoton != null)
        {
            textoBoton.text = "MATA JEFE";
        }
    }

    void Update()
    {
        // Atajo de teclado
        if (Input.GetKeyDown(KeyCode.F9))
        {
            MataJefeYAbrePuerta();
        }
    }
}