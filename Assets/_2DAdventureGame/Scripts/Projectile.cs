using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("More things to try: прибирання снарядів")]
    [Tooltip("Скільки секунд живе снаряд, якщо ні в що не влучив")]
    public float lifeTime = 2.0f;

    Rigidbody2D rigidbody2d;
    float lifeTimer;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        lifeTimer = lifeTime;
    }

    void Update()
    {
        // Базовий варіант з туторіалу: далеко від центру світу
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }

        // Інший спосіб (More things to try): таймер, як у невразливості
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Fix();
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
