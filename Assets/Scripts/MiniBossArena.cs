using UnityEngine;
using System.Collections.Generic;

public class MiniBossArena : MonoBehaviour
{
    [Header("CONFIGURACIÓN ARENA")]
    public string nombreArena = "Arena del Mini Boss";

    [Header("REFERENCIAS - ¡ASIGNAR ESTOS!")]
    public List<GameObject> puertasEntrada = new List<GameObject>();
    public PuertaFinal puertaFinal;
    public Transform spawnPointJefe;

    [Header("PREFAB JEFE - ¡ASIGNAR ESTE!")]
    public GameObject prefabMiniJefe;

    [Header("COMPONENTES VISUALES Y SPAWN")]
    public MiniJefeSpawner spawnerJefe;
    public bool usarSpawnerExterno = false;

    [Header("RECOMPENSAS")]
    public int corazonesRecompensa = 3;

    [Header("DEBUG")]
    public bool debugMode = true;

    private bool arenaActiva = false;
    private bool arenaCompletada = false;
    private GameObject miniJefeInstance;
    private MiniJefeVisual visualJefe;
    private HealthManager healthManager;

    void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
        if (healthManager == null)
        {
            Debug.LogError("❌ HealthManager no encontrado en la escena");
        }
        else
        {
            if (debugMode) Debug.Log("✅ HealthManager encontrado");
        }

        VerificarConfiguracion();
        CerrarPuertasEntrada(false);

        if (spawnerJefe == null && !usarSpawnerExterno)
        {
            ConfigurarSpawnerAutomatico();
        }

        if (debugMode) Debug.Log($"✅ {nombreArena} - Inicializada");
    }

    void ConfigurarSpawnerAutomatico()
    {
        GameObject spawnerObj = new GameObject("Spawner_MiniBoss");
        spawnerObj.transform.SetParent(transform);
        spawnerObj.transform.position = spawnPointJefe != null ? spawnPointJefe.position : transform.position;

        spawnerJefe = spawnerObj.AddComponent<MiniJefeSpawner>();
        spawnerJefe.prefabMiniJefe = prefabMiniJefe;
        spawnerJefe.spawnPoint = spawnPointJefe;
        spawnerJefe.spawnAlIniciar = false;
        spawnerJefe.debugMode = debugMode;

        if (debugMode) Debug.Log("🔧 Spawner automático configurado");
    }

    void VerificarConfiguracion()
    {
        bool configuracionCorrecta = true;

        if (prefabMiniJefe == null)
        {
            Debug.LogError("❌ FALTA: Prefab Mini Jefe no asignado en el inspector");
            configuracionCorrecta = false;
        }

        if (spawnPointJefe == null)
        {
            Debug.LogError("❌ FALTA: Spawn Point Jefe no asignado en el inspector");
            configuracionCorrecta = false;
        }

        if (healthManager == null)
        {
            Debug.LogError("❌ HealthManager no encontrado - Las recompensas no funcionarán");
            configuracionCorrecta = false;
        }

        if (configuracionCorrecta)
        {
            Debug.Log("🎯 Configuración de la arena - COMPLETA");
        }
        else
        {
            Debug.LogError("🚨 CONFIGURACIÓN INCOMPLETA - Revisa los errores arriba");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !arenaCompletada && !arenaActiva)
        {
            if (prefabMiniJefe != null && spawnPointJefe != null)
            {
                IniciarArena();
            }
            else
            {
                Debug.LogError("🚨 No se puede iniciar arena - Faltan referencias esenciales");
            }
        }
    }

    void IniciarArena()
    {
        arenaActiva = true;
        CerrarPuertasEntrada(true);
        SpawnearMiniJefe();

        if (debugMode) Debug.Log($"🎯 {nombreArena} - ACTIVADA");
    }

    void SpawnearMiniJefe()
    {
        if (usarSpawnerExterno && spawnerJefe != null)
        {
            spawnerJefe.SpawnearMiniJefe();
            miniJefeInstance = spawnerJefe.GetComponentInChildren<MiniJefe>()?.gameObject;

            if (debugMode) Debug.Log("🎯 Usando spawner externo para generar jefe");
        }
        else
        {
            if (prefabMiniJefe == null || spawnPointJefe == null)
            {
                Debug.LogError("❌ No se puede spawnear - Faltan referencias");
                return;
            }

            miniJefeInstance = Instantiate(prefabMiniJefe, spawnPointJefe.position, Quaternion.identity);

            visualJefe = miniJefeInstance.GetComponent<MiniJefeVisual>();
            if (visualJefe != null)
            {
                visualJefe.spawnAlActivar = true;
                if (debugMode) Debug.Log("🎨 Componente visual del jefe configurado");
            }

            if (debugMode) Debug.Log($"🪳 Mini jefe spawnedo en: {spawnPointJefe.position}");
        }

        ConfigurarReferenciasJefe();
    }

    void ConfigurarReferenciasJefe()
    {
        if (miniJefeInstance != null)
        {
            MonoBehaviour[] componentes = miniJefeInstance.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in componentes)
            {
                System.Type tipo = comp.GetType();
                var campoArena = tipo.GetField("arena");
                var campoPuerta = tipo.GetField("puertaFinal");

                if (campoArena != null && campoArena.FieldType == typeof(MiniBossArena))
                {
                    campoArena.SetValue(comp, this);
                }

                if (campoPuerta != null && puertaFinal != null)
                {
                    campoPuerta.SetValue(comp, puertaFinal);
                }
            }

            if (debugMode) Debug.Log("🔗 Referencias configuradas en el jefe");
        }
    }

    void Update()
    {
        if (arenaActiva && !arenaCompletada && miniJefeInstance != null)
        {
            // Verificar si el jefe fue destruido o desactivado
            if (!miniJefeInstance.activeInHierarchy)
            {
                MiniJefeDerrotado();
            }

            // Verificar si el jefe tiene un componente que indique que fue derrotado
            MiniJefeVisual visual = miniJefeInstance.GetComponent<MiniJefeVisual>();
            if (visual == null)
            {
                // Si no hay componente visual, usar método alternativo de detección
                // Por ejemplo, verificar si el objeto está a punto de ser destruido
                if (miniJefeInstance == null)
                {
                    MiniJefeDerrotado();
                }
            }
        }
    }

    public void MiniJefeDerrotado()
    {
        if (!arenaCompletada)
        {
            arenaCompletada = true;
            arenaActiva = false;

            DarRecompensas();
            CerrarPuertasEntrada(false);

            if (debugMode) Debug.Log($"🎉 {nombreArena} - COMPLETADA");
        }
    }

    void CerrarPuertasEntrada(bool cerrar)
    {
        foreach (GameObject puerta in puertasEntrada)
        {
            if (puerta != null)
            {
                puerta.SetActive(cerrar);
                Collider2D collider = puerta.GetComponent<Collider2D>();
                if (collider != null) collider.enabled = cerrar;
            }
        }

        if (debugMode) Debug.Log($"🚪 Puertas entrada {(cerrar ? "CERRADAS" : "ABIERTAS")}");
    }

    void DarRecompensas()
    {
        if (healthManager != null)
        {
            // Usar el método Heal del HealthManager
            healthManager.Heal(corazonesRecompensa);
            if (debugMode) Debug.Log($"💚 Recompensa: +{corazonesRecompensa} corazones de curación");
        }
        else
        {
            // Fallback - solo usar la variable estática health
            HealthManager.health = Mathf.Min(HealthManager.health + corazonesRecompensa, 3); // Máximo fijo de 3 corazones
            HealthManager hm = FindObjectOfType<HealthManager>();
            if (hm != null)
            {
                hm.UpdateHearts();
                if (debugMode) Debug.Log($"💚 Recompensa aplicada (fallback): +{corazonesRecompensa} corazones");
            }
            else
            {
                Debug.LogError("❌ No se pudo aplicar recompensa - HealthManager no encontrado");
            }
        }
    }

    [ContextMenu("🔄 Reiniciar Arena")]
    public void ReiniciarArena()
    {
        if (miniJefeInstance != null)
        {
            Destroy(miniJefeInstance);
            if (debugMode) Debug.Log("🗑️ Jefe anterior eliminado");
        }

        if (spawnerJefe != null)
        {
            spawnerJefe.EliminarJefe();
        }

        arenaActiva = false;
        arenaCompletada = false;

        CerrarPuertasEntrada(false);

        if (debugMode) Debug.Log("🔄 Arena reiniciada");
    }

    [ContextMenu("💖 Dar Recompensa Manual")]
    public void DarRecompensaManual()
    {
        DarRecompensas();
        Debug.Log("💖 Recompensa dada manualmente");
    }

    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = arenaActiva ? Color.red : (arenaCompletada ? Color.green : Color.yellow);
            Gizmos.DrawWireCube(transform.position, collider.size);
        }

        if (spawnPointJefe != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(spawnPointJefe.position, 0.5f);
            Gizmos.DrawWireCube(spawnPointJefe.position, new Vector3(1, 1, 0));

#if UNITY_EDITOR
            UnityEditor.Handles.Label(spawnPointJefe.position + Vector3.up * 0.7f, "🪳 Spawn Boss");
#endif
        }
    }
}