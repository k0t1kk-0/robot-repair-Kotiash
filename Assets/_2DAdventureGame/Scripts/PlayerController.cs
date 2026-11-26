using System.Collections;
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

    [Header("Feedback (лабораторна 9)")]
    public AudioClip hitClip;
    public AudioClip launchClip;
    public Color hitFlashColor = new Color(1f, 0.35f, 0.35f);
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;

    [Header("Dialogue")]
    public InputAction TalkAction;
    private NonPlayerCharacter lastNonPlayerCharacter;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

        // Промінь: чи дивиться робот на NPC
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            if (lastNonPlayerCharacter != null && lastNonPlayerCharacter != npc)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
            }
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend();
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
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
            PlaySound(hitClip);
            StopCoroutine(nameof(HitFlash));
            StartCoroutine(nameof(HitFlash));
        }

        // Обмежуємо здоров'я в межах [0, maxHealth]
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, projectileForce);

        animator.SetTrigger("Launch");
        PlaySound(launchClip);
    }

    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue(lastNonPlayerCharacter.dialogueText);
        }
    }

    // Один звук на подію: шкода щокадру відсікається невразливістю, тому і звук не гуде
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator HitFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
