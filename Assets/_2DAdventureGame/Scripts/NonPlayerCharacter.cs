using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;

    [TextArea]
    [Tooltip("Що каже цей NPC")]
    public string dialogueText;

    void Start()
    {
        dialogueBubble.SetActive(false);
    }
}
