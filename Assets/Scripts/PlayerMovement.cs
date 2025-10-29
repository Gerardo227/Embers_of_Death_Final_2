using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpForce = 15f;
    public bool isFacingRight = true;

    [Header("Components")]
    public Rigidbody2D playerRB;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;

    // Private variables
    private bool isGrounded;
    private int numberOfJumps = 0;
    private float horizontalInput;

    void Start()
    {
        Debug.Log("🔄 PlayerMovement.Start() iniciado");

        // Verificar componentes
        if (playerRB == null)
        {
            Debug.LogError("❌ PlayerMovement: playerRB no asignado!");
        }
        else
        {
            Debug.Log("✅ PlayerMovement: playerRB asignado correctamente");
        }

        if (groundCheck == null)
        {
            Debug.LogError("❌ PlayerMovement: groundCheck no asignado!");
        }
        else
        {
            Debug.Log($"✅ PlayerMovement: groundCheck asignado en posición {groundCheck.position}");
        }

        Debug.Log($"🎯 PlayerMovement: Configuración - Speed: {speed}, Jump: {jumpForce}");
    }

    void Update()
    {
        // INPUT
        float oldInput = horizontalInput;
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != oldInput && horizontalInput != 0)
        {
            Debug.Log($"🎮 PlayerMovement: Input detectado - Horizontal: {horizontalInput}");
        }

        // DETECTAR SUELO
        bool oldGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded != oldGrounded)
        {
            Debug.Log($"🏠 PlayerMovement: Grounded cambiado a {isGrounded}");
        }

        // SALTO
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log($"🦘 PlayerMovement: Tecla de salto presionada - Grounded: {isGrounded}, Saltos: {numberOfJumps}");

            if (isGrounded)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
                numberOfJumps = 1;
                Debug.Log("✅ PlayerMovement: Salto terrestre ejecutado");
            }
            else if (numberOfJumps == 1)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
                numberOfJumps = 2;
                Debug.Log("✅ PlayerMovement: Doble salto ejecutado");
            }
            else
            {
                Debug.Log("❌ PlayerMovement: No puede saltar - sin saltos disponibles");
            }
        }

        // FLIP
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }

        // ANIMACIONES
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    void FixedUpdate()
    {
        // VERIFICAR GAME OVER
        if (PlayerManager.isGameOver)
        {
            Debug.Log("⛔ PlayerMovement: Game Over - Movimiento bloqueado");
            playerRB.velocity = new Vector2(0, playerRB.velocity.y);
            return;
        }

        // MOVIMIENTO
        Vector2 oldVelocity = playerRB.velocity;
        playerRB.velocity = new Vector2(horizontalInput * speed, playerRB.velocity.y);

        if (horizontalInput != 0 && oldVelocity.x != playerRB.velocity.x)
        {
            Debug.Log($"🚀 PlayerMovement: Movimiento aplicado - Input: {horizontalInput}, Velocidad X: {playerRB.velocity.x}");
        }
    }

    void Flip()
    {
        Debug.Log("🔄 PlayerMovement: Volteando personaje");
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🔵 PlayerMovement: Colisión entrante con {collision.gameObject.name} (Layer: {collision.gameObject.layer})");

        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer))
        {
            numberOfJumps = 0;
            isGrounded = true;
            Debug.Log("✅ PlayerMovement: Saltos reseteados - tocando suelo");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"🔴 PlayerMovement: Colisión saliente con {collision.gameObject.name}");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
            Debug.Log("🌫️ PlayerMovement: Dejó de tocar el suelo");
        }
    }
}