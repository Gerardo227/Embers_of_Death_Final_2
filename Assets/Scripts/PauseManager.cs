using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseScreen;

    // ✅ REFERENCIAS MANUALES
    public Button resumeButton;
    public Button retryButton;
    public Button menuButton;
    public Button quitButton;

    private void Awake()
    {
        Debug.Log("🔄 PauseManager.Awake() iniciado");
        isGamePaused = false;

        if (pauseScreen == null)
        {
            Debug.LogError("❌ PauseManager: pauseScreen es NULL");
        }
        else
        {
            Debug.Log($"✅ PauseManager: pauseScreen asignado - {pauseScreen.name}");
            pauseScreen.SetActive(false);
        }

        SetupButtonsManually();
    }

    void Start()
    {
        SetupButtonsManually();
    }

    private void SetupButtonsManually()
    {
        Debug.Log("🔧 PauseManager: Configurando botones manualmente");

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            Debug.Log($"✅ Botón Resume configurado - {resumeButton.name}");
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryLevel);
            Debug.Log($"✅ Botón Retry configurado - {retryButton.name}");
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
            Debug.Log($"✅ Botón Menu configurado - {menuButton.name}");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitToMenu);
            Debug.Log($"✅ Botón Quit configurado - {quitButton.name}");
        }
    }

    void Update()
    {
        // ✅ NO PERMITIR PAUSA MIENTRAS EL LEVEL COMPLETE ESTÉ ACTIVO
        if (LevelCompleteManager.isLevelComplete) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("⌨️ PauseManager: Tecla de pausa detectada");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isGamePaused) return;

        Debug.Log("⏸️ PauseManager: Pausando juego");
        isGamePaused = true;
        Time.timeScale = 0f;

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(true);
            Debug.Log("✅ PauseManager: UI de pausa activado");
        }
    }

    public void ResumeGame()
    {
        if (!isGamePaused) return;

        Debug.Log("▶️ PauseManager: Reanudando juego");
        isGamePaused = false;
        Time.timeScale = 1f;

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
            Debug.Log("🚫 PauseManager: UI de pausa desactivado");
        }
    }

    // ✅ ACTUALIZADO: RESETEAR POSICIÓN AL REINICIAR
    public void RetryLevel()
    {
        Debug.Log("🔄 PauseManager: Reiniciando nivel desde pausa");

        Time.timeScale = 1f;
        isGamePaused = false;

        // ✅ RESETEAR POSICIÓN ANTES DE RECARGAR
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.ResetToInitialPosition();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Debug.Log("🏠 PauseManager: Yendo al menú principal");

        Time.timeScale = 1f;
        isGamePaused = false;

        SceneManager.LoadScene(0);
    }

    public void QuitToMenu()
    {
        Debug.Log("🚪 PauseManager: Saliendo al menú principal");

        Time.timeScale = 1f;
        isGamePaused = false;

        SceneManager.LoadScene(0);
    }

    [ContextMenu("Verificar Configuración")]
    public void CheckConfiguration()
    {
        Debug.Log("🔍 PauseManager: Verificación de configuración:");
        Debug.Log($"🎯 isGamePaused: {isGamePaused}");
        Debug.Log($"🎯 Time.timeScale: {Time.timeScale}");
    }

    private void OnDestroy()
    {
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (retryButton != null) retryButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
    }
}