using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    public static bool isLevelComplete = false;
    public GameObject levelCompleteScreen;

    private bool levelCompleteUIShown = false;

    private void Awake()
    {
        Debug.Log("🔄 LevelCompleteManager.Awake() iniciado");
        isLevelComplete = false;
        levelCompleteUIShown = false;

        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(false);
            Debug.Log("✅ LevelCompleteManager: UI desactivado al inicio");
        }
    }

    void Update()
    {
        if (isLevelComplete && !levelCompleteUIShown)
        {
            ShowLevelCompleteUI();
        }
    }

    public void CompleteLevel()
    {
        if (!isLevelComplete)
        {
            Debug.Log("🎉 LevelCompleteManager: Nivel completado!");
            isLevelComplete = true;
        }
    }

    private void ShowLevelCompleteUI()
    {
        Debug.Log("🏁 LevelCompleteManager: Mostrando UI de Nivel Completado");
        levelCompleteUIShown = true;

        if (levelCompleteScreen == null)
        {
            Debug.LogError("❌ LevelCompleteManager: levelCompleteScreen es NULL");
            return;
        }

        levelCompleteScreen.SetActive(true);
        Debug.Log("✅ LevelCompleteManager: UI de nivel completado activado");
    }

    // ✅ ACTUALIZADO: RESETEAR POSICIÓN AL REINICIAR
    public void RetryLevel()
    {
        Debug.Log("🔄 LevelCompleteManager: Botón Reintentar PRESIONADO");

        Time.timeScale = 1f;
        PauseManager.isGamePaused = false;
        isLevelComplete = false;
        levelCompleteUIShown = false;

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
        Debug.Log("🏠 LevelCompleteManager: Botón Menú PRESIONADO");

        Time.timeScale = 1f;
        PauseManager.isGamePaused = false;
        isLevelComplete = false;
        levelCompleteUIShown = false;

        SceneManager.LoadScene("Menu");
    }

    [ContextMenu("Test Level Complete")]
    public void TestLevelComplete()
    {
        Debug.Log("🧪 LevelCompleteManager: Probando nivel completado");
        CompleteLevel();
    }
}