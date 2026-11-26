using System.Collections;
using UnityEngine;

// Механіка-твіст: коли гравець наступив на плиту, двері відчиняються на openTime секунд.
public class TimedSwitch : MonoBehaviour
{
    public GameObject door;
    public float openTime = 4.0f;

    [Header("Зворотний зв'язок")]
    public Color idleColor = new Color(0.55f, 0.85f, 0.55f);
    public Color activeColor = new Color(1f, 0.85f, 0.3f);

    SpriteRenderer plate;

    void Start()
    {
        plate = GetComponent<SpriteRenderer>();
        if (plate != null) plate.color = idleColor;
        door.SetActive(true);
    }

    // Твіст вимикається галочкою Active / enabled — тоді двері просто відчинені
    void OnDisable()
    {
        if (door != null) door.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(OpenDoorFor(openTime));
    }

    IEnumerator OpenDoorFor(float seconds)
    {
        door.SetActive(false);
        if (plate != null) plate.color = activeColor;

        // More things to try: залишок часу на HUD
        float left = seconds;
        while (left > 0)
        {
            if (UIHandler.instance != null) UIHandler.instance.SetSwitchTimer(left);
            left -= Time.deltaTime;
            yield return null;
        }

        if (UIHandler.instance != null) UIHandler.instance.SetSwitchTimer(0);
        if (plate != null) plate.color = idleColor;
        door.SetActive(true);
    }
}
