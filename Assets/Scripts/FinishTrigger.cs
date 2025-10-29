using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigurableFinishTrigger : MonoBehaviour
{
    [Header("Opciones de Finalización")]
    public bool goToMenu = true;
    public bool showWinMessage = true;

    [Header("Mensaje Personalizado")]
    public string winMessage = "¡Nivel Completado!";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (showWinMessage)
            {
                Debug.Log($"🎉 {winMessage}");
            }

            if (goToMenu)
            {
                Debug.Log("🏁 Cargando menú principal...");
                SceneManager.LoadScene(0);
            }
        }
    }
}