using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;

    [Header("Movement")]
    public float speed = 3.0f;

    [Header("Health")]
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    [Header("Invincibility")]
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public InputAction LaunchAction;
    public float projectileForce = 300.0f;

    [Tooltip("More things to try: пауза між пострілами (с)")]
    public float timeBetweenLaunches = 0.5f;
    float launchCooldown;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Встановлюємо повне здоров'я на старті
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // Відлік таймера невразливості
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Відлік паузи між пострілами
        if (launchCooldown > 0)
        {
            launchCooldown -= Time.deltaTime;
        }

        if (LaunchAction.WasPressedThisFrame() && launchCooldown <= 0)
        {
            Launch();
            launchCooldown = timeBetweenLaunches;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        // Якщо отримуємо шкоду (amount < 0)
        if (amount < 0)
        {
            if (isInvincible)
            {
                return; // Якщо вже невразливі — шкоду не зараховуємо
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        // Обмежуємо здоров'я в межах [0, maxHealth]
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, projectileForce);

        animator.SetTrigger("Launch");
    }
}
