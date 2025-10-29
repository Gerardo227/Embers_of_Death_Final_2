using UnityEngine;

public class Campfire : MonoBehaviour
{
    [Header("Campfire Settings")]
    public bool isActive = false;
    public int healAmount = 50;
    public ParticleSystem fireEffect;
    public Light campfireLight;
    
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        SetCampfireState(isActive);
    }
    
    public void Interact()
    {
        if (!isActive)
        {
            ActivateCampfire();
        }
        else
        {
            RestAtCampfire();
        }
    }
    
    void ActivateCampfire()
    {
        isActive = true;
        SetCampfireState(true);
        
        // Guardar juego
        SaveManager.Instance.SetCheckpointPosition(transform.position);
        Debug.Log("Fogata activada!");
    }
    
    void RestAtCampfire()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.RefillEstus();
            player.GetComponent<HealthSystem>().Heal(healAmount);
            Debug.Log("Descansando en la fogata...");
        }
    }
    
    void SetCampfireState(bool active)
    {
        if (animator != null) animator.SetBool("IsActive", active);
        if (fireEffect != null) 
        {
            if (active) fireEffect.Play();
            else fireEffect.Stop();
        }
        if (campfireLight != null) campfireLight.enabled = active;
    }
}