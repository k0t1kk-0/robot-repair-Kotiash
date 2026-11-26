using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    [Header("More things to try: випадкова зміна осі")]
    [Tooltip("Якщо увімкнено, при кожному розвороті ворог може випадково змінити вісь руху")]
    public bool randomAxis;
    [Range(0f, 1f)]
    [Tooltip("Ймовірність змінити вісь при розвороті")]
    public float axisChangeChance = 0.5f;

    public bool isBroken { get { return broken; } }

    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;
    Vector2 startPosition;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
        startPosition = rigidbody2d.position;    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;

            if (randomAxis && Random.value < axisChangeChance)
            {
                vertical = !vertical;

                // Щоб ворог не «розповзався» по карті, на новій осі йдемо назад до точки старту
                float current = vertical ? rigidbody2d.position.y : rigidbody2d.position.x;
                float start = vertical ? startPosition.y : startPosition.x;
                direction = current > start ? -1 : 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");

        if (UIHandler.instance != null)
        {
            UIHandler.instance.UpdateRobotCounter();
        }
    }
}
