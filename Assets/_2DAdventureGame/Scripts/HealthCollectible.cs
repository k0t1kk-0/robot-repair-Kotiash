using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [Tooltip("Кількість здоров'я, яку відновлює ця аптечка")]
    public int amount = 1;

    [Tooltip("Звук підбирання (лабораторна 9)")]
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            // Передаємо значення змінної amount замість фіксованої одиниці
            controller.ChangeHealth(amount);
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }
}