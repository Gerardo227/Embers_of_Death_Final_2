using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static bool isGameOver = false;
    public GameObject gameOverScreen;
    public GameObject pauseMenuScreen;

    public static Vector2 lastCheckPointPos = new Vector2(10f, 15f);
    public static int numberOfCoins = 0;

    public GameObject[] playerPrefabs;
    private GameObject currentPlayer;
    private bool gameOverUIShown = false;

    private void Awake()
    {
        Debug.Log("🔄 PlayerManager.Awake() iniciado");
        isGameOver = false;
        gameOverUIShown = false;

        // ✅ RESETEO SEGURO DE POSICIÓN
        ResetCheckpointIfNeeded();

        if (gameOverScreen == null)
        {
            Debug.LogError("❌ PlayerManager: gameOverScreen es NULL desde el inicio");
        }
        else
        {
            Debug.Log($"✅ PlayerManager: gameOverScreen asignado - Nombre: {gameOverScreen.name}");
            gameOverScreen.SetActive(false);
            Debug.Log("🚫 PlayerManager: gameOverScreen desactivado al inicio");
        }

        // Spawn del jugador
        SpawnPlayer();

        Debug.Log("🎮 PlayerManager: Awake completado");
    }

    private void ResetCheckpointIfNeeded()
    {
        // ✅ SOLO RESETEAR SI LA POSICIÓN ACTUAL ES PELIGROSA (debajo del death plane)
        if (lastCheckPointPos.y < -50f) // Ajusta este valor según tu death plane
        {
            Debug.Log($"⚠️ Posición de checkpoint peligrosa: {lastCheckPointPos} - Reseteando a posición inicial");
            lastCheckPointPos = new Vector2(10f, 15f);
        }

        Debug.Log($"📍 Checkpoint inicial: {lastCheckPointPos}");
    }

    private void SpawnPlayer()
    {
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Debug.Log($"🎯 PlayerManager: Spawneando personaje índice: {characterIndex} en posición: {lastCheckPointPos}");

        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("❌ PlayerManager: playerPrefabs es NULL o VACÍO");
            return;
        }

        if (characterIndex < 0 || characterIndex >= playerPrefabs.Length)
        {
            characterIndex = 0;
            Debug.LogWarning($"⚠️ PlayerManager: Índice fuera de rango. Usando 0.");
        }

        if (playerPrefabs[characterIndex] == null)
        {
            Debug.LogError($"❌ PlayerManager: Prefab en índice {characterIndex} es NULL");
            return;
        }

        // ✅ DESTRUIR JUGADOR ANTERIOR SI EXISTE
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Destroy(existingPlayer);
            Debug.Log("🗑️ PlayerManager: Jugador anterior destruido");
        }

        currentPlayer = Instantiate(playerPrefabs[characterIndex], lastCheckPointPos, Quaternion.identity);
        Debug.Log($"✅ PlayerManager: Personaje instanciado: {currentPlayer.name} en {lastCheckPointPos}");

        numberOfCoins = PlayerPrefs.GetInt("NumberOfCoins", 0);
        Debug.Log($"💰 PlayerManager: Monedas: {numberOfCoins}");
    }

    void Start()
    {
        Debug.Log("🔛 PlayerManager.Start() iniciado");
        Debug.Log($"🎯 PlayerManager: isGameOver en Start: {isGameOver}");
        Debug.Log($"🎯 PlayerManager: gameOverUIShown en Start: {gameOverUIShown}");

        if (gameOverScreen != null)
        {
            Debug.Log($"✅ PlayerManager: gameOverScreen verificado - activo: {gameOverScreen.activeInHierarchy}");
        }
    }

    void Update()
    {
        if (isGameOver && !gameOverUIShown)
        {
            ShowGameOverUI();
        }
    }

    // ✅ MÉTODO PARA RESETEAR POSICIÓN (usado al reiniciar)
    public void ResetToInitialPosition()
    {
        Debug.Log("🔄 PlayerManager: Resetando a posición inicial");
        lastCheckPointPos = new Vector2(10f, 15f);
        Debug.Log($"✅ Posición reseteada a: {lastCheckPointPos}");
    }

    // ✅ MÉTODO PARA CHECKPOINTS (compatible con tu script)
    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        lastCheckPointPos = newCheckpoint;
        Debug.Log($"📍 Checkpoint actualizado: {lastCheckPointPos}");

        // ✅ VERIFICAR QUE EL CHECKPOINT SEA SEGURO
        if (lastCheckPointPos.y < -10f) // Ajusta según tu nivel
        {
            Debug.LogWarning($"⚠️ Checkpoint en posición potencialmente peligrosa: {lastCheckPointPos}");
        }
    }

    private void ShowGameOverUI()
    {
        Debug.Log("💀 PlayerManager: Mostrando UI de Game Over");
        gameOverUIShown = true;

        if (gameOverScreen == null)
        {
            Debug.LogError("❌ PlayerManager: gameOverScreen es NULL");
            return;
        }

        gameOverScreen.SetActive(true);
        Debug.Log("✅ PlayerManager: gameOverScreen activado");
    }

    public void ReplayLevel()
    {
        Debug.Log("🔁 PlayerManager: Reiniciando nivel");

        // ✅ RESETEAR POSICIÓN ANTES DE REINICIAR
        ResetToInitialPosition();

        isGameOver = false;
        gameOverUIShown = false;
        HealthManager.health = 3;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Debug.Log("⏸️ PlayerManager: Juego pausado");
        Time.timeScale = 0;
        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(true);
            Debug.Log("✅ PlayerManager: Menú de pausa activado");
        }
    }

    public void ResumeGame()
    {
        Debug.Log("▶️ PlayerManager: Juego reanudado");
        Time.timeScale = 1;
        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(false);
            Debug.Log("✅ PlayerManager: Menú de pausa desactivado");
        }
    }

    public void GoToMenu()
    {
        Debug.Log("🏠 PlayerManager: Yendo al menú");

        // ✅ RESETEAR POSICIÓN ANTES DE IR AL MENÚ
        ResetToInitialPosition();

        isGameOver = false;
        gameOverUIShown = false;
        HealthManager.health = 3;
        SceneManager.LoadScene("Menu");
    }

    // ✅ MÉTODOS DE DEBUG
    [ContextMenu("Test Reset Position")]
    public void TestResetPosition()
    {
        ResetToInitialPosition();
    }

    [ContextMenu("Verificar Checkpoint Actual")]
    public void VerificarCheckpoint()
    {
        Debug.Log($"📍 Checkpoint actual: {lastCheckPointPos}");
        Debug.Log($"🎯 isGameOver: {isGameOver}");
        Debug.Log($"💰 Monedas: {numberOfCoins}");
    }
}