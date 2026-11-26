using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private Label m_DialogueLabel;
    private float m_TimerDisplay;

    private VisualElement m_Healthbar;

    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;

    // More things to try: лічильник «Полагоджено x / N»
    private Label m_RobotCounter;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_DialogueLabel = m_NonPlayerDialogue.Q<Label>();
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

        m_RobotCounter = uiDocument.rootVisualElement.Q<Label>("RobotCounter");
        UpdateRobotCounter();
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    // More things to try: у кожного NPC своя репліка
    public void DisplayDialogue(string text)
    {
        if (m_DialogueLabel != null && !string.IsNullOrEmpty(text))
        {
            m_DialogueLabel.text = text;
        }
        DisplayDialogue();
    }

    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
    }

    public void UpdateRobotCounter()
    {
        if (m_RobotCounter == null) return;

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        int fixedCount = 0;
        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.isBroken) fixedCount++;
        }
        m_RobotCounter.text = "Полагоджено " + fixedCount + " / " + enemies.Length;
    }
}
